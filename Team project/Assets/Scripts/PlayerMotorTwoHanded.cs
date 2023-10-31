using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMotorTwoHanded : MonoBehaviour
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

    private bool IsHoldingRifle = false;
    private bool IsHoldingShotgun = false;

    public GameObject grenadePrefab;
    public Transform grenadeSpawnPoint;
    public float throwForce = 10f;
    public float grenadeThrowDelay = 1.0f;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        healthManager = GetComponent<PlayerHealthManager>();
        soundManager = SoundManager.instance;

        gunFires = GetComponent<GunFires>();
        if (gunFires == null)
        {
            Debug.LogError("GunFires script not found on the same GameObject.");
        }
    }



    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, groundMask);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isSprinting ? speed * sprintSpeedMultiplier : speed;


        if (Input.GetKeyDown(KeyCode.G))
        {
            animator.SetTrigger("IsThrowing");
            Invoke("ThrowGrenade", grenadeThrowDelay);
        }

        if (IsWalkingOrRunningForward && Input.GetKeyDown(KeyCode.C) && IsGrounded)
        {
            animator.SetTrigger("IsRunDiving");
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
                gunFires.SetAimingState(true);
                animator.SetBool("IsAiming", true);

                Controller.enabled = false;
            }
            else
            {
                gunFires.SetAimingState(false);
                animator.SetBool("IsAiming", false);

                Controller.enabled = true;
            }
        }

        bool allowMovement = !animator.GetBool("IsAiming");

        if (allowMovement)
        {
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
            }

            if (Input.GetKey(KeyCode.A) && IsGrounded)
            {
                animator.SetBool("IsRunningLeft", true);
                gunFires.SetAimingState(false);
            }
            else
            {
                animator.SetBool("IsRunningLeft", false);
                gunFires.SetAimingState(true);
            }

            if (Input.GetKey(KeyCode.D) && IsGrounded)
            {
                animator.SetBool("IsRunningRight", true);
                gunFires.SetAimingState(false);
            }
            else
            {
                animator.SetBool("IsRunningRight", false);
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
            if (IsIdle || Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !StrafeLeft && !StrafeRight)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                animator.SetTrigger("IsJumping");
            }
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
            }
        }
    }
}
