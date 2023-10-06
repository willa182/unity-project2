using UnityEngine;

public class EnemyBullets : MonoBehaviour
{
    public int damage = 1; // Adjust the damage amount as needed.

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.gameObject.GetComponent<PlayerHealthManager>();

            if (playerHealth != null)
            {
                playerHealth.HurtPlayer(damage);
            }

            Destroy(gameObject);
        }
    }
}