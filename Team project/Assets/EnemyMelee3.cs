using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee3 : MonoBehaviour
{
    public float walkPointRange = 10f;
    public float sightRange = 5f;
    public float attackRange = 2f;
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float attackCooldown = 2f; // Cooldown time after an attack

    private Transform player;
    private NavMeshAgent agent;
    private Vector3 walkPoint;
    private bool playerInSightRange;
    private bool playerInAttackRange;
    private bool isAttacking = false;
    private bool cooldownActive = false;

    Animator animator;
    public bool isRandomWalkEnabled = true;
    private bool isAnimatorEnabled = false;

    private float timePlayerEnteredCollider; // Track the time when the player entered the collider
    private bool isStandingUp = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        animator = GetComponent<Animator>();
        agent.enabled = false;
        StartCoroutine(RandomWalk());
    }

    void Update()
    {
        playerInSightRange = Vector3.Distance(transform.position, player.position) <= sightRange;
        playerInAttackRange = Vector3.Distance(transform.position, player.position) <= attackRange;

        if (cooldownActive)
        {
            // Enemy is on cooldown, don't move or attack
            return;
        }

        if (isAnimatorEnabled)
        {
            if (!isStandingUp && Time.time - timePlayerEnteredCollider >= 0.5f)
            {
                // Set the "IsStandingUp" animation to true 2 seconds after the player entered
                animator.SetTrigger("IsStandingUp");
                isStandingUp = true;
                StartCoroutine(EnableNavMeshAgentAfterDelay(3.7f));
            }
        }


        if (playerInSightRange && !playerInAttackRange)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", true);
            animator.ResetTrigger("Melee"); // Reset the attack trigger.
            agent.speed = runSpeed;
            agent.SetDestination(player.position);
        }
        else if (playerInAttackRange)
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackPlayer());
            }
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsWalking", true);
            animator.ResetTrigger("Melee"); // Reset the attack trigger.
            agent.speed = walkSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAnimatorEnabled = true;
            timePlayerEnteredCollider = Time.time;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isAnimatorEnabled = false;
        }
    }

    IEnumerator EnableNavMeshAgentAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.enabled = true;
    }

    IEnumerator RandomWalk()
    {
        while (isRandomWalkEnabled) // Check the flag
        {
            if (!playerInSightRange)
            {
                animator.SetBool("IsWalking", true);
                Vector3 randomDirection = Random.insideUnitSphere * walkPointRange;
                randomDirection += transform.position;
                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkPointRange, 1);
                walkPoint = hit.position;
                agent.SetDestination(walkPoint);

                yield return new WaitForSeconds(Random.Range(4f, 8f));
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;
        animator.SetBool("IsRunning", false);
        animator.SetBool("IsWalking", false);

        // Determine the attack type based on random chances
        float randomChance = Random.Range(0f, 1f);

        if (randomChance <= 0.4f)
        {
            // 40% chance to trigger the "IsKicking" animation
            animator.SetTrigger("IsKicking");
        }
        else if (randomChance <= 0.2f)
        {
            // 25% chance to trigger the "IsHeadbutting" animation
            animator.SetTrigger("IsHeadbutting");
        }
        else
        {
            // Default attack animation or any other logic you have
            animator.SetTrigger("Melee");
        }

        agent.speed = 0f; // Stop moving
        FaceTarget(); // Make the enemy face the player

        // Start cooldown as soon as the attack animation is triggered
        cooldownActive = true;
        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        cooldownActive = false; // Reset cooldown flag
    }

    void FaceTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void ApplyDamageToPlayer()
    {
        int damageAmount = 10;

        PlayerHealthManager playerHealthManager = player.GetComponent<PlayerHealthManager>();

        if (playerHealthManager != null)
        {
            playerHealthManager.HurtPlayer(damageAmount);
        }
    }
}
