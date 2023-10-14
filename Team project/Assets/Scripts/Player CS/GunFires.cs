using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFires : MonoBehaviour
{
    public Transform firePoint;
    public float bulletForce = 10f;
    public Animator playerAnimator;
    public float mouseSensitivity = 2.0f;

    public GameObject bulletPrefab; 

    private bool isIdle = true;
    private bool isAiming = false;
    private PlayerLook playerLook;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerLook = GetComponent<PlayerLook>();
        if (playerLook == null)
        {
            Debug.LogError("PlayerLook script not found on the same GameObject.");
        }

        SetAimingState(false);
    }

    void Update()
    {
        if (isAiming && Input.GetButtonDown("Fire1"))
        {
            Debug.Log("is shooting");
            Shoot();
            playerAnimator.SetTrigger("IsShooting");
            SetAimingState(false);
        }
        else
        {
            bool canShootWithoutAiming = !isAiming && IsMoving();

            if (canShootWithoutAiming && Input.GetButtonDown("Fire1"))
            {
                Debug.Log("is shooting without aiming");
                Shoot();
                playerAnimator.SetTrigger("IsShooting");
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Before Instantiate: " + bulletPrefab.name);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (bullet != null)
        {
            Debug.Log("Bullet instantiated successfully");

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

    IEnumerator DestroyBulletDelayed(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }


    public void SetIdleState(bool isIdleState)
    {
        isIdle = isIdleState;
    }

    public void SetAimingState(bool isAimingState)
    {
        isAiming = isAimingState;
        playerLook.SetAiming(isAiming);
    }

    private bool IsMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
