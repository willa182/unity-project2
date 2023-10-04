using System.Collections;
using UnityEngine;

public class GunFire : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletForce = 10f;
    public float mouseSensitivity = 2.0f;
    public Animator playerAnimator;

    private Transform playerTransform;
    private bool isAiming = false;
    private bool isRightMouseButtonDown = false;

    void Start()
    {
        playerTransform = transform;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        playerTransform.Rotate(Vector3.up, mouseX);


        if (Input.GetButtonDown("Fire2"))
        {
            isRightMouseButtonDown = true;
            if (!isAiming)
            {
                Debug.Log("is aiming");
                playerAnimator.SetBool("Aim", true);
                isAiming = true;
            }
        }

      
        if (Input.GetButtonUp("Fire2"))
        {
            isRightMouseButtonDown = false;
        }


   
        if (!isRightMouseButtonDown && isAiming)
        {
            Debug.Log("is not aiming");
            playerAnimator.SetBool("Aim", false);
            isAiming = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            playerAnimator.SetTrigger("Shoot");
        }
    }

    void Shoot()
    {
        if (isAiming)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(firePoint.forward * bulletForce, ForceMode.Impulse);
            }

            Destroy(bullet, 2.0f);
        }
    }
}
