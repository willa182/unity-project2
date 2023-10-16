using System.Collections;
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

    private bool IsIdle;
    public float sprintSpeedMultiplier = 1.5f;

    Animator animator;
    GunFires gunFires;

    private bool Craft; // For walking
    private bool Move;  // For running
    private bool StrafeLeft;
    private bool StrafeRight;

    private bool canPickup = true;
    private bool isPickupAnimationPlaying = false;

    private PlayerHealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        healthManager = GetComponent<PlayerHealthManager>();

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
        Move = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && stamina > 0;

        float currentSpeed = isSprinting ? speed * sprintSpeedMultiplier : speed;

        animator.SetBool("IsRunning", Move && isSprinting);

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
                animator.SetBool("Melee", true);
                Invoke("ResetMeleeFlag", 1f);
            }
            if (Input.GetKeyDown(KeyCode.E) && !isPickupAnimationPlaying && canPickup)
            {
                TryPickupAnimation();
            }

            StrafeLeft = Input.GetKey(KeyCode.A);
            animator.SetBool("IsStrafingLeft", StrafeLeft);

            StrafeRight = Input.GetKey(KeyCode.D);
            animator.SetBool("IsStrafingRight", StrafeRight);

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
            }
            else
            {
                RegenerateStamina();
            }
        }

        UpdateStaminaUI();
    }

    void TryPickupAnimation()
    {
        PlayerInventory playerInventory = GetComponent<PlayerInventory>();

        if (playerInventory != null && playerInventory.CanPickUp())
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            bool canPickup = false;

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("WeaponPickup"))
                {
                    canPickup = true;
                    break;
                }
            }

            if (canPickup)
            {
                Debug.Log("PickingUp animation triggered.");
                animator.SetBool("IsPickingUp", true);
                isPickupAnimationPlaying = true;
                Controller.enabled = false;

                // Coroutine to reset pickup flags and continue the animation
                StartCoroutine(playerInventory.ResetPickupFlag());
            }
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
            if ((IsIdle || (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) && !StrafeLeft && !StrafeRight)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                animator.SetBool("IsJumping", true);
            }
            else
            {
                animator.SetBool("IsJumping", false);
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
        if (!Move && stamina < 100f)
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
        }
    }
}
