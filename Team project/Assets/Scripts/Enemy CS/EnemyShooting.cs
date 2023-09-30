using System.Collections;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletForce = 10f;
    public float shootingRange = 10f;
    public float shootingCooldown = 1f;

    private Transform player;
    private bool canShoot = true;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; 
    }

    void Update()
    {
    
        RotateTowardsPlayer();

        if (canShoot && Vector3.Distance(transform.position, player.position) <= shootingRange)
        {
            Shoot();
        }
    }

    void RotateTowardsPlayer()
    {
       
        Vector3 direction = (player.position - transform.position).normalized;

      
        if (direction != Vector3.zero)
        {
        
            Quaternion lookRotation = Quaternion.LookRotation(direction);

          
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 90f);
        }
    }

    void Shoot()
    {
        canShoot = false;
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = transform.forward * bulletForce; 
        Destroy(bullet, 2f); 
        StartCoroutine(EnableShootingCooldown());
    }

    IEnumerator EnableShootingCooldown()
    {
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }
}
