using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explosionRadius = 5f;
    public float explosionForce = 10f;
    public float damage = 50f;

    private bool exploded = false;
    public GameObject explosionEffectPrefab;

    private SoundManager soundManager;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
        soundManager = SoundManager.instance;
    }

    IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(3.0f); // Adjust the delay time as needed

        if (!exploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;

        // Find all colliders in the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            EnemyHealthManager enemyHealth = hit.GetComponent<EnemyHealthManager>();

            if (enemyHealth != null)
            {
                // Calculate the damage based on the distance from the explosion center
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damagePercent = Mathf.Clamp01(1 - distance / explosionRadius);
                int damageAmount = (int)(damage * damagePercent);

                // Apply explosion damage to the enemy
                enemyHealth.TakeExplosionDamage(damageAmount);

            }
            PlayerHealthManager playerHealth = hit.GetComponent<PlayerHealthManager>();

            if (playerHealth != null)
            {
                // Calculate the damage based on the distance from the explosion center
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                float damagePercent = Mathf.Clamp01(1 - distance / explosionRadius);
                int damageAmount = (int)(damage * damagePercent);

                // Apply explosion damage to the player
                playerHealth.HurtPlayer(damageAmount);
            }
        }

        // Instantiate and play the explosion effect
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            soundManager.PlayGrenadeExplo();
        }

        // Destroy the grenade after exploding
        Destroy(gameObject);
    }
}
