using UnityEngine;

public class PlayerMeleeRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            PlayerMotor playerMotor = GetComponentInParent<PlayerMotor>();
            playerMotor.enemyHealthManager = other.GetComponent<EnemyHealthManager>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            PlayerMotor playerMotor = GetComponentInParent<PlayerMotor>();
            playerMotor.enemyHealthManager = null;
        }
    }
}
