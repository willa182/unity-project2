using System.Collections;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletForce = 10f;
    public float shootingRange = 10f;
    public float shootingCooldown = 1f;
    public Transform firePoint;

    private Transform player;
    private bool canShoot = true;
    private PlayerHealthManager playerHealth; 

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (firePoint == null)
        {
            Debug.LogError("Fire point not assigned in the inspector!");
        }

        playerHealth = player.GetComponent<PlayerHealthManager>();
    }

    void Update()
    {
        RotateTowardsPlayer();

        if (playerHealth != null && playerHealth.currentHealth > 0 && canShoot && Vector3.Distance(transform.position, player.position) <= shootingRange)
        {
            if (Random.value < 0.35f) // 25% chance to miss
            {
                ShootMiss();
            }
            else
            {
                Shoot();
            }
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
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = bullet.transform.forward * bulletForce;

        Destroy(bullet, 2f);
        StartCoroutine(EnableShootingCooldown());
    }

    void ShootMiss()
    {
        canShoot = false;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent <Rigidbody>();

        // Calculate a random direction for the miss (left or right)
        Vector3 missDirection = Quaternion.Euler(0, Random.Range(-25f, 25f), 0) * transform.forward;
        rb.velocity = missDirection * bulletForce;

        Destroy(bullet, 2f);
        StartCoroutine(EnableShootingCooldown());
    }


    IEnumerator EnableShootingCooldown()
    {
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }
}
