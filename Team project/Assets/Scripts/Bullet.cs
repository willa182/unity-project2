using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;  // Set the damage of each bullet in the inspector

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("PlayerBullet") && other.CompareTag("Enemy"))
        {
            DealDamage(other.gameObject.GetComponent<EnemyHealth>());
        }
        else if (gameObject.CompareTag("EnemyBullet") && other.CompareTag("Player"))
        {
            DealDamage(other.gameObject.GetComponent<PlayerHealth>());
        }

        // Destroy the bullet upon collision with any object
        Destroy(gameObject);
    }

    private void DealDamage(IDamageable target)
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }
    }
}
