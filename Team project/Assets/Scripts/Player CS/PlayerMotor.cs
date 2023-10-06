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

    Animator animator;

    private bool Craft; // For walking
    private bool Move;  // For running
    private bool StrafeLeft; 
    private bool StrafeRight;

    private bool isPickupAnimationPlaying = false;

    private PlayerHealthManager healthManager;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        healthManager = GetComponent<PlayerHealthManager>();
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, groundMask);

        Move = Input.GetKey(KeyCode.LeftShift) && stamina > 0;

        animator.SetBool("IsRunning", Move);

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetBool("Melee", true);
            Invoke("ResetMeleeFlag", 1f);
        }
        if (Input.GetKeyDown(KeyCode.E) && !isPickupAnimationPlaying)
        {
            animator.SetBool("PickingUp", true);
            isPickupAnimationPlaying = true;

            Controller.enabled = false;

            Invoke("ResetPickupFlag", 3f);
        }

        Craft = Input.GetKey(KeyCode.W) && IsGrounded;
        animator.SetBool("IsWalking", Craft);

        Craft = Input.GetKey(KeyCode.S) && IsGrounded;
        animator.SetBool("IsWalkingBWD", Craft);

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

        UpdateStaminaUI();
    }


    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        float currentSpeed = Move ? speed * 1.5f : speed;

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
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private void DrainStamina()
    {
        if (stamina > 0)
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
        animator.SetBool("Melee", false);
    }

    void ResetPickupFlag()
    {
        animator.SetBool("PickingUp", false);
        isPickupAnimationPlaying = false;

        Controller.enabled = true;
    }
}
