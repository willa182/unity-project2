using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float openSpeed = 2.0f; // Speed at which the door opens.
    public float closeSpeed = 2.0f; // Speed at which the door closes.
    private bool isOpen = false; // Tracks the state of the door.
    private bool isMoving = false; // Checks if the door is already moving.

    private void Update()
    {
        if (isMoving)
        {
            // If the door is currently moving, continue moving it.
            if (isOpen)
                OpenDoor();
            else
                CloseDoor();
        }
        else
        {
            // If the player is inside the trigger and presses 'E', toggle the door state.
            if (Input.GetKeyDown(KeyCode.E))
            {
                isMoving = true;
                isOpen = !isOpen;
            }
        }
    }

    private void OpenDoor()
    {
        // Calculate the new rotation of the door when opening.
        Quaternion targetRotation = Quaternion.Euler(0, 90, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);

        // If the door is fully open, stop moving.
        if (Quaternion.Angle(transform.localRotation, targetRotation) < 1.0f)
        {
            isMoving = false;
        }
    }

    private void CloseDoor()
    {
        // Calculate the new rotation of the door when closing.
        Quaternion targetRotation = Quaternion.identity;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * closeSpeed);

        // If the door is fully closed, stop moving.
        if (Quaternion.Angle(transform.localRotation, targetRotation) < 1.0f)
        {
            isMoving = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player has entered the trigger area, so they can interact with the door.
            // Display a message to the player, or perform any other interactions you need.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Player has exited the trigger area, so they can no longer interact with the door.
            // Hide any messages or UI elements related to the door interaction.
        }
    }
}

