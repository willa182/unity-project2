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
    private PlayerHealthManager playerHealthManager;

    private bool isAttacking = false; // Track if the enemy is currently attacking.
    private float attackCooldown = 3.0f; // Time between attacks
    private float lastAttackTime;

    public int damageAmount = 10;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealthManager = FindObjectOfType<PlayerHealthManager>();
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

                if (distanceToPlayer <= attackRange && !isAttacking && Time.time - lastAttackTime >= attackCooldown)
                {
                    // Stop the enemy.
                    navMeshAgent.isStopped = true;
                    animator.SetBool("IsRunning", false);
                    animator.SetBool("IsWalking", false);

                    // Set the Melee trigger for attacking.
                    animator.SetTrigger("Melee");

                    // Set the attacking state.
                    Debug.Log("Attacking is set to true");
                    isAttacking = true;
                    lastAttackTime = Time.time;
                }
                else if (distanceToPlayer > attackRange && isAttacking)
                {
                    // If the player is out of attack range and we were attacking, reset states.
                    navMeshAgent.isStopped = false;
                    isAttacking = false;

                    // Set the running animation to true when not attacking
                    animator.SetBool("IsRunning", true);
                }
                if (!isAttacking)
                {
                    navMeshAgent.SetDestination(playerTransform.position);
                }
            }
            else
            {
                // Player is out of chase range
                animator.SetBool("IsRunning", false);
                navMeshAgent.speed = walkSpeed;
                animator.SetBool("Melee", false); // Ensure Melee is disabled when not in attack range.
                isAttacking = false; // Reset the attacking state.

                // Set the idle animation to true when not attacking and not in chase range
                animator.SetBool("IsIdle", true);
            }
        }
    }

    void FacePlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // Called by Animation Event in the attack animation
    public void AttackPlayer()
    {
        if (isAttacking)
        {
            // Check if the player health manager is available
            if (playerHealthManager != null)
            {
                // Call the HurtPlayer method in the player's health manager
                playerHealthManager.HurtPlayer(damageAmount);
            }

            // Set the last attack time and reset the attack state
            lastAttackTime = Time.time;
            isAttacking = false;
        }
    }
}
