using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        soundManager = SoundManager.instance;

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
                if (Input.GetButtonDown("Fire1") && IsAutomaticWeapon())
                {
                    StartCoroutine(AutomaticFire());
                }
                else if (Input.GetButtonDown("Fire1"))
                {
                    StartCoroutine(ShootWithDelay());
                }
            }
            else if (IsMoving() && Input.GetButtonDown("Fire1"))
            {
                StartCoroutine(ShootWithDelay());
            }
        }
    }

    IEnumerator ShootWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Adjust the delay time as needed
        PlayWeaponFireSound();
        Shoot();
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
        yield return new WaitForSeconds(0.1f); // Adjust the delay time as needed
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

    IEnumerator AutomaticFire()
    {
        while (Input.GetButton("Fire1") && IsAutomaticWeapon())
        {
            PlayWeaponFireSound();
            Shoot();
            playerAnimator.SetTrigger("IsShooting");
            yield return null;
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
