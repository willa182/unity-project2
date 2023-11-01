using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController Controller;
    public GameObject groundCheck;
    public float groundCheckRadius = 0.2f;
    private Vector3 playerVelocity;
    private bool IsGrounded;
    public LayerMask groundMask;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 2f;
    public float stamina = 100f;
    public float staminaRegenRate = 5f;
    public float staminaDepletionRate = 10f;
    public Slider staminaSlider;
    public Transform rightHand;
    private float lastPressTime;

    private bool IsIdle;
    public float sprintSpeedMultiplier = 1.5f;
    private bool IsWalkingOrRunningForward;

    Animator animator;
    GunFires gunFires;

    private bool Craft; // For walking
    private bool Move;  // For running
    private bool StrafeLeft;
    private bool StrafeRight;

    private bool canPickup = true;
    private SoundManager soundManager;

    private PlayerHealthManager healthManager;
    private bool IsHoldingTwoHandedWeapon = false;
    private bool IsTwoHandedWeaponAnimationActive = false;

    private bool IsHoldingRifle = false;
    private bool IsHoldingShotgun = false;

    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint; 
    public float throwForce = 10f;
    public float grenadeThrowDelay = 1.0f;

    public int availableGrenades = 0;
    public PlayerInventory playerInventory;

    public delegate void GrenadeThrown();
    public static event GrenadeThrown OnGrenadeThrown;

    private AmmoManager ammoManager;
    public EnemyHealthManager enemyHealthManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        healthManager = GetComponent<PlayerHealthManager>();
        soundManager = SoundManager.instance;
        playerInventory = GetComponent<PlayerInventory>();

        gunFires = GetComponent<GunFires>();
        if (gunFires == null)
        {
            Debug.LogError("GunFires script not found on the same GameObject.");
        }

        ammoManager = FindObjectOfType<AmmoManager>();
        if (ammoManager == null)
        {
            Debug.LogError("AmmoManager not found in the scene.");
        }
    }

    void CheckEquippedWeapons()
    {
        IsHoldingRifle = false;
        IsHoldingShotgun = false;

        Collider[] colliders = rightHand.GetComponentsInChildren<Collider>(true);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Rifle"))
            {
                IsHoldingRifle = true;
                IsHoldingTwoHandedWeapon = true;
            }
            else if (collider.CompareTag("Shotgun"))
            {
                IsHoldingShotgun = true;
                IsHoldingTwoHandedWeapon = true;
            }
        }
    }

    public void UpdateAvailableGrenades(int newGrenadeCount)
    {
        availableGrenades = newGrenadeCount;
    }


    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, groundMask);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? speed * sprintSpeedMultiplier : speed;

        CheckEquippedWeapons();

        if (IsHoldingRifle || IsHoldingShotgun)
        {
            // The player is holding a Rifle or Shotgun, so set the "TwoHandedWeapon" parameter.
            animator.SetBool("TwoHandedWeapon", true);
        }
        else
        {
            // The player is not holding a Rifle or Shotgun, so set the "TwoHandedWeapon" parameter to false.
            animator.SetBool("TwoHandedWeapon", false);
        }


        if (Input.GetKeyDown(KeyCode.G))
        {
            // Check if the player has grenades before throwing.
            if (availableGrenades > 0)
            {
                animator.SetTrigger("IsThrowing");
                Invoke("ThrowGrenade", grenadeThrowDelay);

                // Decrement the available grenades count.
       //         availableGrenades--;
                playerInventory.UpdateGrenadeCountText();
       //         OnGrenadeThrown?.Invoke();
            }
            else
            {
                // Play a sound or provide feedback that the player is out of grenades.
            }
        }

        if (IsWalkingOrRunningForward && Input.GetKeyDown(KeyCode.C) && IsGrounded)
        {
            if (IsHoldingTwoHandedWeapon)
            {
                animator.SetTrigger("IsDiving2");
            }
            else
            {
                animator.SetTrigger("IsDiving");
            }
        }

        // Check if the player is walking or running forward
        IsWalkingOrRunningForward = Input.GetKey(KeyCode.W) || (Input.GetKey(KeyCode.W) && isSprinting);


        if (Input.GetKey(KeyCode.W) && IsGrounded && isSprinting)
        {
            animator.SetBool("IsRunning", true);
            DrainStamina();
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Regenerate stamina when not sprinting
        if (!isSprinting && stamina < 100f)
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S) && IsGrounded && isSprinting)
        {
            animator.SetBool("IsRunningBackwards", true);
            DrainStamina();
        }
        else
        {
            animator.SetBool("IsRunningBackwards", false);
        }

        // Regenerate stamina when not sprinting
        if (!isSprinting && stamina < 100f)
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.W) && IsGrounded)
        {
            animator.SetBool("IsWalking", true);
            gunFires.SetAimingState(false);
        }
        else
        {
            animator.SetBool("IsWalking", false);
            if (IsIdle && Input.GetButton("Fire2"))
            {
                if (IsHoldingTwoHandedWeapon)
                {
                    animator.SetBool("IsAiming2", true);
                    gunFires.SetAimingState(true);
                }
                else
                {
                    animator.SetBool("IsAiming", true);
                    gunFires.SetAimingState(true);
                }

                Controller.enabled = false;
            }
            else
            {
                if (IsHoldingTwoHandedWeapon)
                {
                    animator.SetBool("IsAiming2", false);
                    gunFires.SetAimingState(false);
                }
                else
                {
                    animator.SetBool("IsAiming", false);
                    gunFires.SetAimingState(false);
                }

                Controller.enabled = true;
            }
        }
        bool allowMovement = !animator.GetBool("IsAiming");

        if (allowMovement)
        {
            if (Input.GetKey(KeyCode.S) && IsGrounded)
            {
                animator.SetBool("isWalkingBackward", true);
                gunFires.SetAimingState(false);
            }
            else
            {
                animator.SetBool("isWalkingBackward", false);
                gunFires.SetAimingState(true);
            }

            IsIdle = !Move && !Craft && !StrafeLeft && !StrafeRight;
            gunFires.SetIdleState(IsIdle);

            if (Input.GetKeyDown(KeyCode.F) && IsIdle)
            {
                if (Time.time - lastPressTime < 0.3f) // Adjust the time window (0.3f) for quick succession
                {
                    animator.SetBool("MeleeAlternative", true);
                }
                lastPressTime = Time.time; // Update the last press time
                animator.SetBool("Melee", true);
                Invoke("ResetMeleeFlag", 1f); Invoke("ResetMeleeFlag", 1f);
                DealMeleeDamage();
            }

            if (Input.GetKey(KeyCode.A) && IsGrounded)
            {
                animator.SetBool("IsStrafingLeft", true);
                gunFires.SetAimingState(false);
            }
            else
            {
                animator.SetBool("IsStrafingLeft", false);
                gunFires.SetAimingState(true);
            }

            if (Input.GetKey(KeyCode.D) && IsGrounded)
            {
                animator.SetBool("IsStrafingRight", true);
                gunFires.SetAimingState(false);
            }
            else
            {
                animator.SetBool("IsStrafingRight", false);
                gunFires.SetAimingState(true);
            }

            if (!IsGrounded)
            {
                animator.SetBool("IsJumping", false);
            }

            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
            {
                Jump();
            }

            if (Move && IsGrounded)
            {
                DrainStamina();

                if (stamina <= 0)
                {
                    soundManager.PlayOutOfBreathSound();
                }
                else if (stamina > 50)
                {
                    soundManager.StopOutOfBreathSound();
                }
            }
            else
            {
                RegenerateStamina();
            }
        }

        UpdateStaminaUI();
    }
    
   public void DealMeleeDamage()
    {
        if (enemyHealthManager != null) // Check if an enemy is in range
        {
            StartCoroutine(DealMeleeDamageWithDelay(50, 0.7f)); // Change the delay duration (1.0f) as needed
        }
    }

    IEnumerator DealMeleeDamageWithDelay(int damageAmount, float delay)
    {
        yield return new WaitForSeconds(delay);

        enemyHealthManager.HurtEnemy(damageAmount);
        enemyHealthManager.healthBar.gameObject.SetActive(true);

        enemyHealthManager.healthBar.maxValue = enemyHealthManager.health;
    }


    void TryPickupWeapon()
    {
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();

        if (playerInventory != null && playerInventory.CanPickUp())
        {
            StartCoroutine(playerInventory.PickupProcess());
        }
    }


    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? speed * sprintSpeedMultiplier : speed;

        Controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        if (IsGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        Controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (IsGrounded)
        {
            if (IsHoldingTwoHandedWeapon)
            {
                animator.SetTrigger("IsJumping2");
            }
            else
            {
                animator.SetTrigger("IsJumping");
            }
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    private void DrainStamina()
    {
        if (animator.GetBool("IsRunning") && stamina > 0)
        {
            stamina -= staminaDepletionRate * Time.deltaTime;
        }
    }

    private void RegenerateStamina()
    {
        if (!Input.GetKey(KeyCode.LeftShift) && stamina < 100f)
        {
            stamina += staminaRegenRate * Time.deltaTime;
        }
    }

    private void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            float staminaPercent = stamina / 100f;
            staminaSlider.value = staminaPercent;
        }
    }

    void ResetMeleeFlag()
    {
        if (IsIdle)
        {
            animator.SetBool("Melee", false);
            animator.SetBool("MeleeAlternative", false);
        }
    }

    void ThrowGrenade()
    {
        if (grenadePrefab != null && grenadeSpawnPoint != null)
        {
            GameObject grenade = Instantiate(grenadePrefab, grenadeSpawnPoint.position, grenadeSpawnPoint.rotation);

            Rigidbody grenadeRigidbody = grenade.GetComponent<Rigidbody>();
            if (grenadeRigidbody != null)
            {
                // Calculate the forward direction based on the player's rotation
                Vector3 throwDirection = transform.forward;

                // Apply force to the grenade to control its trajectory
                grenadeRigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);

                // Decrement the available grenades count.
                availableGrenades--;

                // Call the DecrementGrenadeCount method in PlayerInventory to update grenade count.
                playerInventory.DecrementGrenadeCount();
            }
        }
    }
}
