using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class GunFires : MonoBehaviour
{
    public Transform firePoint;
    public float bulletForce = 10f;
    public Animator playerAnimator;
    public float mouseSensitivity = 2.0f;

    public Transform playerHand; // Reference to the player's hand
    private bool isIdle = true;
    private bool isAiming = false;
    private PlayerLook playerLook;

    public GameObject pistolBulletPrefab;
    public GameObject rifleBulletPrefab;
    public GameObject shotgunBulletPrefab;
    private GameObject currentBulletPrefab;

    private SoundManager soundManager;
    private bool isFiring = false;

    public AmmoManager ammoManager;
    public Text ammoCountText;
    Animator animator;

    public float pistolFireRate = 0.5f; // Adjust these values to your desired fire rates
    public float shotgunFireRate = 1.0f;
    public float rifleFireRate = 1.0f;
    private float lastPistolShotTime;
    private float lastShotgunShotTime;
    private float lastRifleShotTime;

    private bool canFire = false;
    public GameObject pistolParticles; // Reference to the Pistol Particle System GameObject
    public GameObject rifleParticles; // Reference to the Rifle Particle System GameObject
    public GameObject shotgunParticles;


    void Start()
    {
        if (pistolParticles != null)
            pistolParticles.SetActive(false);

        if (rifleParticles != null)
            rifleParticles.SetActive(false);

        if (shotgunParticles != null)
            shotgunParticles.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        soundManager = SoundManager.instance;
        ammoManager = GameObject.Find("AmmoManager").GetComponent<AmmoManager>();

        animator = gameObject.GetComponent<Animator>();

        playerLook = GetComponent<PlayerLook>();
        if (playerLook == null)
        {
            Debug.LogError("PlayerLook script not found on the same GameObject.");
        }

        SetAimingState(false);
    }

    void Update()
    {
        if (IsValidWeaponInHand())
        {
            bool isAimingNow = Input.GetMouseButton(1);

            if (IsMoving() || isAimingNow)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    string currentWeapon = GetCurrentWeaponType();
                    UpdateAmmoText();

                    if (currentWeapon == "Rifle" && IsAutomaticWeapon() && ammoManager.CanShoot(currentWeapon))
                    {
                        if (Time.time > GetLastShotTime(currentWeapon) + GetFireRate(currentWeapon))
                        {
                            Debug.Log("Firing automatic weapon.");
                            StartCoroutine(AutomaticFire(currentWeapon));
                            UpdateLastShotTime(currentWeapon);

                            if (rifleParticles != null)
                            {
                                rifleParticles.SetActive(true);
                                ParticleSystem rifleParticleSystem = rifleParticles.GetComponent<ParticleSystem>();
                                if (rifleParticleSystem != null)
                                {
                                    rifleParticleSystem.Play();
                                }
                            }
                        }
                    }
                    else if (CanFire(currentWeapon))
                    {
                        Debug.Log("Firing single shot with " + currentWeapon);

                        // Deduct ammo only from the main ammo
                        if (ammoManager.GetAmmoCount(currentWeapon) > 0)
                        {
                            StartCoroutine(ShootWithDelay(currentWeapon));
                            PlayWeaponFireSound();
                            ammoManager.Shoot(currentWeapon);
                            UpdateAmmoText();
                            UpdateLastShotTime(currentWeapon);

                            if (currentWeapon == "Pistol" && pistolParticles != null)
                            {
                                pistolParticles.SetActive(true);
                                ParticleSystem pistolParticleSystem = pistolParticles.GetComponent<ParticleSystem>();
                                if (pistolParticleSystem != null)
                                {
                                    pistolParticleSystem.Play();
                                }
                            }

                            if (currentWeapon == "Shotgun" && shotgunParticles != null)
                            {
                                shotgunParticles.SetActive(true);
                                ParticleSystem shotgunParticleSystem = shotgunParticles.GetComponent<ParticleSystem>();
                                if (shotgunParticleSystem != null)
                                {
                                    shotgunParticleSystem.Play();
                                }
                            }
                        }
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.R) && IsValidWeaponInHand())
            {
                string currentWeapon = GetCurrentWeaponType();
                ReloadWeapon(currentWeapon);
            }

            // Check if the ammo count is 0 and automatically reload
            if (ammoManager.GetAmmoCount(GetCurrentWeaponType()) == 0)
            {
                string currentWeapon = GetCurrentWeaponType();
                ReloadWeapon(currentWeapon);
            }

            canFire = !IsMoving() || !Input.GetMouseButton(1);

            SetAimingState(isAimingNow);
        }
    }

    public string GetCurrentWeaponType()
    {
        foreach (Transform weaponTransform in playerHand)
        {
            if (weaponTransform.CompareTag("Pistol"))
            {
                return "Pistol";
            }
            else if (weaponTransform.CompareTag("Rifle"))
            {
                return "Rifle";
            }
            else if (weaponTransform.CompareTag("Shotgun"))
            {
                return "Shotgun";
            }
        }
        return ""; // Return an empty string if no valid weapon is found
    }

    public void UpdateAmmoText()
    {
        if (ammoCountText != null && ammoManager != null)
        {
            // Retrieve the current equipped weapon type
            string currentWeaponType = GetCurrentWeaponType();

            // Get the corresponding ammo count from the AmmoManager
            int ammoCount = ammoManager.GetAmmoCount(currentWeaponType);

            // Update the UI Text with the ammo count
            ammoCountText.text = "Ammo: " + ammoCount;
        }
    }


    IEnumerator ShootWithDelay(string currentWeapon)
    {
        // Delay before shooting
        yield return new WaitForSeconds(0.2f);

        switch (currentWeapon)
        {
            case "Pistol":
                currentBulletPrefab = pistolBulletPrefab;
                break;
            case "Rifle":
                currentBulletPrefab = rifleBulletPrefab;
                break;
            case "Shotgun":
                currentBulletPrefab = shotgunBulletPrefab;
                break;
            default:
                // Handle the case where the current weapon is unknown or not handled
                break;
        }

        if (currentBulletPrefab != null)
        {
            StartCoroutine(InstantiateBulletWithDelay());
        }

        playerAnimator.SetTrigger("IsShooting");
    }

    private bool CanFire(string currentWeapon)
    {
        float lastShotTime = GetLastShotTime(currentWeapon);
        float fireRate = GetFireRate(currentWeapon);
        return Time.time - lastShotTime >= fireRate;
    }

    private void UpdateLastShotTime(string currentWeapon)
    {
        if (currentWeapon == "Pistol")
        {
            lastPistolShotTime = Time.time;
        }
        else if (currentWeapon == "Shotgun")
        {
            lastShotgunShotTime = Time.time;
        }
        else if (currentWeapon == "Rifle")
        {
            lastRifleShotTime = Time.time;
        }
    }

    private float GetLastShotTime(string currentWeapon)
    {
        if (currentWeapon == "Pistol")
        {
            return lastPistolShotTime;
        }
        else if (currentWeapon == "Shotgun")
        {
            return lastShotgunShotTime;
        }
        else if (currentWeapon == "Rifle")
        {
            return lastRifleShotTime;
        }
        return 0.0f;
    }

    private float GetFireRate(string currentWeapon)
    {
        if (currentWeapon == "Pistol")
        {
            return pistolFireRate;
        }
        else if (currentWeapon == "Shotgun")
        {
            return shotgunFireRate;
        }
        else if (currentWeapon == "Rifle")
        {
            return rifleFireRate;
        }
        return 0.0f;
    }

    void PlayWeaponFireSound()
    {
        foreach (Transform weaponTransform in playerHand)
        {
            // Check the tag to determine which weapon is firing
            if (weaponTransform.CompareTag("Pistol"))
            {
                soundManager.PlayPistolFireSound();
            }
            else if (weaponTransform.CompareTag("Rifle"))
            {
                soundManager.PlayRifleFireSound();
                Debug.Log("Playing Rifle firing sound"); // Add this line for debugging
            }
            else if (weaponTransform.CompareTag("Shotgun"))
            {
                soundManager.PlayShotgunFireSound();
            }
            // Add more checks for other weapon tags if needed
        }
    }

    void Shoot(string currentWeapon)
    {
        // Deduct ammo only from the main ammo
        if (ammoManager.GetAmmoCount(currentWeapon) > 0)
        {
            StartCoroutine(InstantiateBulletWithDelay());
            playerAnimator.SetTrigger("IsShooting");
            UpdateAmmoText();
            PlayWeaponFireSound();
            ammoManager.Shoot(currentWeapon);
        }
    }

    void ReloadWeapon(string currentWeapon)
    {
        if (ammoManager.CanReload(currentWeapon))
        {
            int currentAmmoCount = ammoManager.GetAmmoCount(currentWeapon);
            int availableAmmo = ammoManager.GetAmmoReserve(currentWeapon);
            int ammoNeeded = GetMaxAmmoCount(currentWeapon) - currentAmmoCount; // Use the maximum ammo count for the specific weapon type

            if (availableAmmo >= ammoNeeded)
            {
                // Reload the weapon only if main ammo is not full
                if (currentAmmoCount < GetMaxAmmoCount(currentWeapon))
                {
                    // Set the "IsReloading" trigger in the animator
                    animator.SetTrigger("IsReloading");


                    // Reload the weapon
                    ammoManager.Reload(currentWeapon, ammoNeeded);
                    UpdateAmmoText();

                    PlayReloadSound(currentWeapon);
                }
            }
            else
            {
                Debug.Log("Not enough reserve ammo to reload.");
            }
        }
    }

    void PlayReloadSound(string currentWeapon)
    {
        switch (currentWeapon)
        {
            case "Pistol":
                soundManager.PlayPistolReload(); // Call the appropriate sound function for reloading
                break;
            case "Rifle":
                soundManager.PlayRifleReload(); // Call the appropriate sound function for reloading
                break;
            case "Shotgun":
                soundManager.PlayShotgunReload(); // Call the appropriate sound function for reloading
                break;
                // Add more cases for other weapon types if needed
        }
    }

    int GetMaxAmmoCount(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                return 15;
            case "Shotgun":
                return 8;
            case "Rifle":
                return 30;
            default:
                return 0;
        }
    }

    bool IsValidWeaponInHand()
    {
        // Check if there's at least one valid weapon in the player's hand
        foreach (Transform weaponTransform in playerHand)
        {
            if (weaponTransform.CompareTag("Pistol") || weaponTransform.CompareTag("Rifle") || weaponTransform.CompareTag("Shotgun"))
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator InstantiateBulletWithDelay()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject bullet = Instantiate(currentBulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet != null)
        {
            Rigidbody rb = bullet.GetComponent <Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("Rigidbody not found on the bullet");
            }

            StartCoroutine(DestroyBulletDelayed(bullet, 2.0f));
        }
        else
        {
            Debug.LogError("Bullet instantiation failed");
        }
    }

    bool IsAutomaticWeapon()
    {
        foreach (Transform weaponTransform in playerHand)
        {
            if (weaponTransform.CompareTag("Rifle"))
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator AutomaticFire(string currentWeapon)
    {
        while (IsAutomaticWeapon() && currentWeapon == "Rifle" && Input.GetButton("Fire1") && ammoManager.CanShoot("Rifle"))
        {
            PlayWeaponFireSound();
            Shoot(currentWeapon);
            playerAnimator.SetTrigger("IsShooting");
            ammoManager.Shoot("Rifle");

            // Introduce a delay between shots
            yield return new WaitForSeconds(rifleFireRate);
        }
        StopWeaponFireSound();
    }

    void StopWeaponFireSound()
    {
        foreach (Transform weaponTransform in playerHand)
        {
            if (weaponTransform.CompareTag("Rifle"))
            {
                soundManager.StopRifleFireSound();
            }
        }
    }


    bool IsMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }

    public void SetAimingState(bool isAimingState)
    {
        isAiming = isAimingState;
        playerLook.SetAiming(isAiming);
    }
    public void SetIdleState(bool isIdleState)
    {
        isIdle = isIdleState;
    }

    IEnumerator DestroyBulletDelayed(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
