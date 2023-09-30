using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;  
    public float gravity = 9.81f;  
    public LayerMask groundLayer; 

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isGrounded;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
  
        isGrounded = Physics.Raycast(transform.position, Vector3.down, characterController.height / 2 + 0.1f, groundLayer);

        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = transform.TransformDirection(new Vector3(horizontalInput, 0.0f, verticalInput));
        moveDirection = inputDir * moveSpeed;

    
        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

    
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
