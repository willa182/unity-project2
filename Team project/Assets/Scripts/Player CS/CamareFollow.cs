using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform component.
    public float smoothSpeed = 0.125f; // Adjust this to control the camera's smoothness.
    public float distanceFromPlayer = 5f; // Adjust this to control the camera's distance from the player.
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        // Calculate the desired position of the camera.
        Vector3 desiredPosition = player.position - player.forward * distanceFromPlayer + Vector3.up * offset.y;

        // Smoothly interpolate the camera's position.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Make the camera always look at the player's position.
        transform.LookAt(player.position);
    }
}
