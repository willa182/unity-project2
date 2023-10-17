using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : MonoBehaviour
{
    public float walkingRange = 5f;
    public float idleTime = 2f;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float screamDuration = 2f; // Duration of the scream in seconds

    public float walkSpeed = 3f;  // Adjust the walk speed as needed
    public float runSpeed = 6f;   // Adjust the run speed as needed

    private Transform player;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Start the initial behavior
        StartWalking();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            // Player is in attack range
            SetAnimation("MeleeAttack");
            navMeshAgent.isStopped = true;
        }
        else if (distanceToPlayer < chaseRange)
        {
            // Player is in chase range
            if (!isChasing)
            {
                isChasing = true;
                SetAnimation("Scream");
                Invoke("StartRunAfterScream", screamDuration);
            }
            else
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = runSpeed;
                navMeshAgent.SetDestination(player.position);
            }
        }
        else
        {
            // Player is out of chase range
            if (isChasing)
            {
                isChasing = false;
                StopNavMeshAgent();
                StartWalking();
            }
        }
    }

    void StartRunAfterScream()
    {
        SetAnimation("IsRunning");
        navMeshAgent.SetDestination(player.position); // Ensure the destination is set after the scream
    }

    void SetAnimation(string animationBool)
    {
        // Reset all animation bools
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsIdle", false);
        animator.SetBool("IsRunning", false);
        animator.SetBool("Melee", false);
        animator.SetBool("Scream", false);

        // Set the specified animation bool
        animator.SetBool(animationBool, true);
    }

    void StartWalking()
    {
        // Set a random destination within walking range
        Vector3 randomDestination = RandomNavmeshLocation(walkingRange);
        navMeshAgent.speed = walkSpeed;  // Set walk speed
        navMeshAgent.SetDestination(randomDestination);

        SetAnimation("IsWalking");

        // Schedule the transition to idle after reaching the destination
        Invoke("StartIdle", navMeshAgent.remainingDistance / navMeshAgent.speed);
    }

    void StartIdle()
    {
        SetAnimation("IsIdle");

        // Schedule the transition to walking after idling for a random time
        Invoke("StartWalking", Random.Range(idleTime, idleTime * 2));
    }

    void StopNavMeshAgent()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
    }

    void ActivateNavMeshAgent()
    {
        navMeshAgent.enabled = true;
        navMeshAgent.isStopped = false;
    }

    Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, radius, -1);
        return navHit.position;
    }
}
