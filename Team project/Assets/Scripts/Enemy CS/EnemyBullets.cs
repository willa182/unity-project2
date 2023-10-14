using UnityEngine;

public class EnemyBullets : MonoBehaviour
{
    public int damage = 1;

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
        else if (other.CompareTag("Object"))
        {
            Destroy(gameObject);
        }
    }
}
