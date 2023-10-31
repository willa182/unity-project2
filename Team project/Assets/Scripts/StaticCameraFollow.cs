using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    public Transform target; // The player's Transform to follow

    [Header("Initial Camera Position and Rotation")]
    public Vector3 initialPosition = new Vector3(0f, 2f, -5f); // Initial camera position
    public Vector3 initialRotation = new Vector3(15f, 0f, 0f);  // Initial camera rotation

    public Vector3 offset;   // Offset from the player's position

    void Start()
    {
        // Set the initial position and rotation
        transform.position = initialPosition;
        transform.eulerAngles = initialRotation;
    }

    void Update()
    {
        if (target == null)
        {
            // Make sure you have assigned the target in the Inspector
            Debug.LogWarning("Target is not assigned to the camera.");
            return;
        }

        // Follow the player's position
        Vector3 newPosition = target.position + offset;
        transform.position = newPosition;
    }
}
