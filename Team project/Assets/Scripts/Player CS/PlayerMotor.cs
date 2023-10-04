using System.Collections;
using UnityEngine;

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
    Animator animator;

    
    private bool Craft; // For walking
    private bool Move;  // For running

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded = Physics.CheckSphere(groundCheck.transform.position, groundCheckRadius, groundMask);

        Move = Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("Move", Move);

        if (Input.GetKeyDown(KeyCode.F))
        {
            animator.SetTrigger("Attack");
        }

        Craft = Input.GetKey(KeyCode.W) && IsGrounded;
        animator.SetBool("Craft", Craft);

        // Reset Ride bool if not grounded
        if (!IsGrounded)
        {
            animator.SetBool("Ride", false);
        }
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
            animator.SetBool("Ride", true);
        }
        else
        {
            animator.SetBool("Ride", false);
        }
    }
}
