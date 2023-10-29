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
    public GameObject bulletPrefab;

    private SoundManager soundManager;
    private bool isFiring = false;

    public AmmoManager ammoManager;
    public Text ammoCountText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        soundManager = SoundManager.instance;
        ammoManager = GameObject.Find("AmmoManager").GetComponent<AmmoManager>();

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
            if (isAiming && Input.GetMouseButton(1))
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    string currentWeapon = GetCurrentWeaponType();
                    UpdateAmmoText();

                    if (currentWeapon == "Pistol")
                    {
                        UpdateAmmoText();
                    }
                    else if (currentWeapon == "Rifle")
                    {
                        UpdateAmmoText();
                    }
                    else if (currentWeapon == "Shotgun")
                    {
                        UpdateAmmoText();
                    }

                    if (currentWeapon == "Rifle" && IsAutomaticWeapon() && ammoManager.CanShoot(currentWeapon))
                    {
                        Debug.Log("Firing automatic weapon.");
                        StartCoroutine(AutomaticFire(currentWeapon));
                    }
                    else if (ammoManager.CanShoot(currentWeapon))
                    {
                        Debug.Log("Firing single shot with " + currentWeapon);
                        StartCoroutine(ShootWithDelay(currentWeapon));
                        PlayWeaponFireSound();
                        ammoManager.Shoot(currentWeapon);
                    }
                }
                else if (IsMoving() && Input.GetButtonDown("Fire1"))
                {
                    Debug.Log("Firing while moving.");
                    StartCoroutine(ShootWithDelay(GetCurrentWeaponType()));
                    StartCoroutine(AutomaticFire(GetCurrentWeaponType()));
                    PlayWeaponFireSound();
                }
            }
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

        // Instantiate the bullet
        StartCoroutine(InstantiateBulletWithDelay());

        playerAnimator.SetTrigger("IsShooting");
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
            }
            else if (weaponTransform.CompareTag("Shotgun"))
            {
                soundManager.PlayShotgunFireSound();
            }
            // Add more checks for other weapon tags if needed
        }
    }

    void Shoot()
    {
        StartCoroutine(InstantiateBulletWithDelay());
          
        UpdateAmmoText();                
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet != null)
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
            }
            else
            {
                Debug.LogError("Rigidbody not found on the bullet");
            }

            // Optionally destroy the bullet after some time
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
        while (Input.GetButton("Fire1") && IsAutomaticWeapon() && ammoManager.CanShoot("Rifle"))
        {
            PlayWeaponFireSound();
            Shoot();
            playerAnimator.SetTrigger("IsShooting");
            ammoManager.Shoot("Rifle");
            yield return null;
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
