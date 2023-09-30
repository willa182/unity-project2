using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    public float chaseRange = 15f;
    public float shootingRange = 10f;
    public float movementSpeed = 5f;

    private Transform player;
    private NavMeshAgent navAgent;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootingRange)
        {
            // Player is within shooting range, stop chasing and start shooting.
            isChasing = false;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Player is within chase range but outside shooting range, start chasing.
            isChasing = true;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        navAgent.SetDestination(player.position);
    }
}
