using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset = new Vector3(0f, 2f, -5f); // Offset from the player
    public float smoothSpeed = 5f; // Speed of camera following

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set for the camera.");
            return;
        }

        // Calculate the desired position based on the player's position and rotation
        Vector3 desiredPosition = player.TransformPoint(offset);

        // Smoothly interpolate between the current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Look at the player's position
        transform.LookAt(player);
    }
}
