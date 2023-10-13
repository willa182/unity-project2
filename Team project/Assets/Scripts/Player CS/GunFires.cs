using UnityEngine;

public class GunFires : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 10f;
    public Animator playerAnimator;
    public float mouseSensitivity = 2.0f;

    private bool isIdle = true; // New variable to track if the player is idle
    private bool isAiming = false; // Track if the player is aiming
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
        // Check if the player is aiming before allowing shooting
        if (isAiming)
        {
            // Check if shooting and the button for shooting is pressed
            if (Input.GetButtonDown("Fire1"))
            {
                Debug.Log("is shooting");
                Shoot();
                playerAnimator.SetTrigger("IsShooting");

                // Stop aiming when shooting
                SetAimingState(false);
            }
        }
        else
        {
            // Check if the player can shoot without aiming when moving
            bool canShootWithoutAiming = !isAiming && IsMoving();

            // Check if shooting without aiming and the button for shooting is pressed
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
        }
        Destroy(bullet, 2.0f);
    }

    // Add a public method to set the idle state from other scripts
    public void SetIdleState(bool isIdleState)
    {
        isIdle = isIdleState;
    }

    // Add a public method to set the aiming state from other scripts
    public void SetAimingState(bool isAimingState)
    {
        isAiming = isAimingState;

        // Start aiming or stop aiming based on the new state
        playerLook.SetAiming(isAiming);
    }

    // Helper method to check if the player is moving
    private bool IsMoving()
    {
        return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
    }
}
