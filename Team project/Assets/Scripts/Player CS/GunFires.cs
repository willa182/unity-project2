using UnityEngine;

public class GunFires : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 10f;
    public Animator playerAnimator;
    public float mouseSensitivity = 2.0f;

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
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            if (!isAiming)
            {
                Debug.Log("is aiming");
                playerAnimator.SetBool("IsAiming", true);
                isAiming = true;
                playerLook.SetAiming(true); // Start aiming
            }
        }

        if (Input.GetButtonUp("Fire2"))
        {
            if (isAiming)
            {
                Debug.Log("is not aiming");
                playerAnimator.SetBool("IsAiming", false);
                isAiming = false;
                playerLook.SetAiming(false); // Stop aiming
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            if (isAiming)
            {
                Debug.Log("is shooting");
                Shoot();
                playerAnimator.SetTrigger("IsShooting");
                playerLook.SetAiming(false); // Stop aiming when shooting
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        Destroy(bullet, 2.0f);
    }
}