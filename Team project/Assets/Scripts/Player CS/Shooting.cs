using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Transform firePoint;       
    public GameObject bulletPrefab;   
    public float bulletForce = 10f;   
    public Transform gunPivot;
    public float mouseSensitivity = 2.0f;

    void Update()
    {
 
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;

       
        gunPivot.Rotate(Vector3.up, mouseX * Time.deltaTime);

      
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

       
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = firePoint.forward * bulletForce;


        Destroy(bullet, 2.0f);
    }
}
