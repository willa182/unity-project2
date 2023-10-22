using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float chaseRange = 30f;
    public float attackRange = 2f;
    public float screamRange = 35f;

    private Animator animator;
    private Transform playerTransform;
    private bool isScreaming = false;
    private NavMeshAgent navMeshAgent;
    public LayerMask groundLayer;

    public delegate void EnemyAttackEvent();
    public event EnemyAttackEvent OnEnemyAttack;

    private bool isAttacking = false; // Track if the enemy is currently attacking.

    void Start()
    {
        animator = GetComponent <Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize the NavMeshAgent component.
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = walkSpeed; // Set initial speed.

        StartCoroutine(EnemyBehavior());
    }

    IEnumerator EnemyBehavior()
    {
        while (true)
        {
            yield return StartCoroutine(IdleState());
            yield return StartCoroutine(WalkState());
        }
    }

    IEnumerator IdleState()
    {
        animator.SetBool("IsIdle", true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        animator.SetBool("IsIdle", false);

        if (isAttacking)
        {
            // Trigger the attack event on the enemy
            OnEnemyAttack?.Invoke();
        }
    }

    IEnumerator WalkState()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas);

        // Set the NavMeshAgent's destination.
        navMeshAgent.SetDestination(hit.position);
        navMeshAgent.speed = walkSpeed; // Set the walking speed.

        animator.SetBool("IsWalking", true);
        yield return new WaitForSeconds(Random.Range(3f, 6f));
        animator.SetBool("IsWalking", false);

        if (isAttacking)
        {
            // Trigger the attack event on the enemy
            OnEnemyAttack?.Invoke();
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer < screamRange && distanceToPlayer >= chaseRange)
        {
            // Player is in scream range but not in chase range
            animator.SetBool("Scream", true);
            isScreaming = true;

            // Face the player when screaming
            FacePlayer();

            // Slow down the NavMeshAgent to a stop, don't stop it completely.
            navMeshAgent.speed = 0f;
        }
        else
        {
            // Player is either in chase range or out of scream range
            animator.SetBool("Scream", false);
            isScreaming = false;

            if (distanceToPlayer < chaseRange)
            {
                // If the player is in chase range, set IsRunning animation and run towards the player.
                animator.SetBool("IsRunning", true);
                navMeshAgent.speed = runSpeed;

                if (distanceToPlayer <= attackRange && !isAttacking)
                {
                    // Stop the enemy.
                    navMeshAgent.isStopped = true;
                    animator.SetBool("IsRunning", false);

                    // Play the Melee animation for attacking.
                    animator.SetBool("Melee", true);

                    // Set the attacking state.
                    isAttacking = true;
                }
                else if (distanceToPlayer > attackRange && isAttacking)
                {
                    // If the player is out of attack range and we were attacking, reset states.
                    navMeshAgent.isStopped = false;
                    animator.SetBool("Melee", false);
                    isAttacking = false;
                }

                // Set the destination only if the NavMeshAgent is not already running.
                if (!navMeshAgent.pathPending && !isAttacking)
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                }
            }
            else
            {
                // Player is out of chase range
                animator.SetBool("IsRunning", false);
                navMeshAgent.speed = walkSpeed;
                navMeshAgent.isStopped = false;
                animator.SetBool("Melee", false); // Ensure Melee is disabled when not in attack range.
                isAttacking = false; // Reset the attacking state.

                // Resume following the player when not attacking.
                if (!isAttacking)
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                }
            }

            if (isAttacking)
            {
                // Trigger the attack event on the enemy
                OnEnemyAttack?.Invoke();
            }
        }
    }

    void FacePlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
