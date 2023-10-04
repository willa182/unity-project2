using UnityEngine;

public class PlayerBullets : MonoBehaviour
{
    public int damage = 1; // Adjust the damage amount as needed.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealthManager enemyHealth = other.gameObject.GetComponent<EnemyHealthManager>();

            if (enemyHealth != null)
            {
                enemyHealth.HurtEnemy(damage);
            }

            Destroy(gameObject); // Destroy the bullet when it hits the player.
        }
    }
}