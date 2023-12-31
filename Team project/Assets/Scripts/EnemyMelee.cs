using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMelee : MonoBehaviour
{
    public float walkPointRange = 10f;
    public float sightRange = 5f;
    public float attackRange = 2f;
    public float soundRange = 30f;
    private float timeSinceLastSound = 0f;
    private float soundInterval = 5f;
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float attackCooldown = 2f; // Cooldown time after an attack

    private Transform player;
    private NavMeshAgent agent;
    private Vector3 walkPoint;
    private bool playerInSightRange;
    private bool playerInAttackRange;
    private bool playerInSoundRange;
    private bool isAttacking = false;
    private bool cooldownActive = false;

    Animator animator;
   public bool isRandomWalkEnabled = true;

    private SoundManager soundManager;
    private bool isChasing = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        animator = GetComponent<Animator>();
        StartCoroutine(RandomWalk());

        soundManager = SoundManager.instance;
    }

    void Update()
    {
        playerInSightRange = Vector3.Distance(transform.position, player.position) <= sightRange;
        playerInAttackRange = Vector3.Distance(transform.position, player.position) <= attackRange;
        playerInSoundRange = Vector3.Distance(transform.position, player.position) <= soundRange;

        if (playerInSoundRange && !isChasing)
        {
            Debug.Log("player in sound range");
            timeSinceLastSound += Time.deltaTime;
            if (timeSinceLastSound >= soundInterval)
            {
                soundManager.PlayRandomZombieSound();
                timeSinceLastSound = 0f;
            }
        }

        if (cooldownActive)
        {
            // Enemy is on cooldown, don't move or attack
            return;
        }

        if (playerInSightRange && !playerInAttackRange)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsWalking", true);
            animator.ResetTrigger("Melee"); // Reset the attack trigger.
            agent.speed = runSpeed;
            agent.SetDestination(player.position);

            if (!isChasing)
            {
                isChasing = true;
                soundManager.PlayRandomZombieChaseSound();
            }
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

            if (isChasing)
            {
                isChasing = false;
            }
        }
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
        animator.SetTrigger("Melee"); // Use a trigger to start the attack animation.
        agent.speed = 0f; // Stop moving
        FaceTarget(); // Make the enemy face the player

        // Start cooldown as soon as the Melee animation is triggered
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
