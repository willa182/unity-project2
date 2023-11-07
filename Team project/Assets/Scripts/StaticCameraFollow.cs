using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    public Transform player; // The player's Transform to follow

    [Header("Initial Camera Position and Rotation")]
    public Vector3 initialPosition = new Vector3(0f, 2f, -5f); // Initial camera position
    public Vector3 initialRotation = new Vector3(15f, 0f, 0f);  // Initial camera rotation

    public Vector3 offset;   // Offset from the target's position

    private Transform currentVehicle; // The vehicle the player is currently driving

    void Start()
    {
        // Set the initial position and rotation
        transform.position = initialPosition;
        transform.eulerAngles = initialRotation;
    }

    void Update()
    {
        if (currentVehicle != null)
        {
            // Follow the current vehicle's position
            Vector3 newPosition = currentVehicle.position + offset;
            transform.position = newPosition;
        }
        else if (player != null)
        {
            // Follow the player's position if no active vehicle is available
            Vector3 newPosition = player.position + offset;
            transform.position = newPosition;
        }
        else
        {
            Debug.LogWarning("Player is not assigned to the camera.");
        }
    }

    public void SetCurrentVehicle(Transform vehicle)
    {
        currentVehicle = vehicle;
    }

    public void ClearCurrentVehicle()
    {
        currentVehicle = null;
    }
}
