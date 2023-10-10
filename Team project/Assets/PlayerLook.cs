using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 2.0f;
    public float rotationSmoothness = 5f;

    private Vector3 targetRotation;
    private bool isAiming = false;

    public bool IsAiming
    {
        get { return isAiming; }
    }

    public void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

        targetRotation.y += mouseX;

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * rotationSmoothness);
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }
}
