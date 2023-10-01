using UnityEngine;

public class CamareFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform component.
    public float smoothSpeed = 0.125f; // Adjust this to control the camera's smoothness.
    public float distanceFromPlayer = 5f; // Adjust this to control the camera's distance from the player.

    private Vector3 offset;
    private Quaternion initialRotation;

    private void Start()
    {
        offset = transform.position - player.position;
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Calculate the desired position of the camera.
        Vector3 desiredPosition = player.position - player.forward * distanceFromPlayer + Vector3.up * offset.y;

        // Smoothly interpolate the camera's position.
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Calculate the target rotation on the Y-axis to follow the player's rotation.
        float targetRotationY = player.eulerAngles.y;

        // Create a Quaternion for the Y-axis rotation.
        Quaternion targetRotation = Quaternion.Euler(initialRotation.eulerAngles.x, targetRotationY, initialRotation.eulerAngles.z);

        // Apply the initial X-axis rotation and smoothly interpolate the camera's Y-axis rotation.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);
    }
}
