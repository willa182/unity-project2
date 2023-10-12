using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float distanceFromPlayer = 5f;
    public float initialRotationX = 50f; // If needed, you can adjust this value

    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - player.position;

        // Set the initial rotation on the X-axis
        transform.localRotation = Quaternion.Euler(initialRotationX, 0, 0);

        // Set the desired position and rotation
        transform.position = new Vector3(-30, 15, -11);
        transform.rotation = Quaternion.Euler(40, 0, 0);
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
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
