using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bulletCasingPrefab;
    public Transform casingSpawnPoint;
    public float casingForce = 1f;
    public float fireRate = 1f;
    public float casingDestroyDelay = 30f;

    private float nextFireTime;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Fire()
    {
        // Instantiate bullet casing
        GameObject bulletCasing = Instantiate(bulletCasingPrefab, casingSpawnPoint.position, casingSpawnPoint.rotation);

        // Add force to the bullet casing
        Rigidbody casingRigidbody = bulletCasing.GetComponent<Rigidbody>();
        casingRigidbody.AddForce(casingSpawnPoint.forward * casingForce, ForceMode.Impulse);

        // Wait before destroying the bullet casing
        Destroy(bulletCasing, casingDestroyDelay);
    }
}
