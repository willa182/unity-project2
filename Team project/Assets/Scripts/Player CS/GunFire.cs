using System.Collections;
using System.Collections.Generic;
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

    void Start()
    {
    
        playerTransform = transform;
    }

    void Update()
    {
        
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

       
        playerTransform.Rotate(Vector3.up, mouseX);

    
        if (Input.GetButton("Fire2"))
        {
            if (!isAiming)
            {
                Debug.Log("is aiming");
                playerAnimator.SetBool("IsAiming", true);
                isAiming = true;
            }

          
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (isAiming)
            {
                Debug.Log("is not aiming");
                playerAnimator.SetBool("IsAiming", false);
                isAiming = false;
            }
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
