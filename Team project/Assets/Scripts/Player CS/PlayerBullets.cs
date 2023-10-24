using UnityEngine;

public class PlayerBullets : MonoBehaviour
{
    public int damage = 10; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealthManager enemyHealth = other.gameObject.GetComponent<EnemyHealthManager>();

            if (enemyHealth != null)
            {
                enemyHealth.currentHealth -= damage;
            }

            Destroy(gameObject); 
        }
        else if (other.CompareTag("Object"))
        {
            Destroy(gameObject);
        }
    }
}