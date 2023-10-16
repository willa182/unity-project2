using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeMove : MonoBehaviour
{
    public float chaseRange = 15f;
    public float walkRange = 10f;
    public float scratchRange = 3f;
    public float walkingSpeed = 3f;
    public float runningSpeed = 6f;

    private Transform player;
    private Animator animator;
    private bool isChasing = false;
    private bool isScratching = false;
    private bool isMeleeAttacking = false;
    private bool stopMelee = false; // Flag to stop Melee animation immediately
    private EnemyHealthManager healthManager;
    private PlayerHealthManager playerHealth;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        healthManager = GetComponent<EnemyHealthManager>();
        playerHealth = player.GetComponent<PlayerHealthManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = walkingSpeed; // Set initial speed
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            HandlePlayerDead();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (healthManager != null && healthManager.IsFlashing())
        {
            MoveTowardsPlayer(runningSpeed);
            StopMeleeAttack();
            EnableNavMeshAgent();
            return;
        }

        bool wasScratching = isScratching;
        isScratching = (distanceToPlayer <= scratchRange);

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;

            if (isScratching)
            {
                HandleScratching();
                DisableNavMeshAgent();
            }
            else if (distanceToPlayer <= walkRange)
            {
                HandleWalking();
            }
            else
            {
                HandleRunning();
            }
        }
        else
        {
            isChasing = false;
            HandleIdle();
            EnableNavMeshAgent();
        }

        // Player left scratch range, stop Melee attack and resume normal behavior
        if (!isChasing && !isScratching && wasScratching)
        {
            stopMelee = true; // Set the flag to stop Melee animation immediately
        }
    }

    void HandlePlayerDead()
    {
        isChasing = false;
        SetWalkingAnimation(false);
        SetRunningAnimation(false);
        animator.SetBool("IsIdle", true);
        StopMeleeAttack();
        EnableNavMeshAgent(); // Ensure NavMeshAgent is enabled when the player is dead
    }

    void HandleScratching()
    {
        // Player is within the scratch range
        SetWalkingAnimation(false);
        SetRunningAnimation(false);

        if (!isMeleeAttacking)
        {
            StartMeleeAttack();
        }
    }

    void HandleWalking()
    {
        // Player is within the walk range
        SetWalkingAnimation(true);
        SetRunningAnimation(false);
        MoveTowardsPlayer(walkingSpeed);
        StopMeleeAttack();
        EnableNavMeshAgent();
    }

    void HandleRunning()
    {
        // Player is outside the walk range but within the chase range
        SetWalkingAnimation(false);
        SetRunningAnimation(true);
        MoveTowardsPlayer(runningSpeed);
        StopMeleeAttack();
        EnableNavMeshAgent();
    }

    void HandleIdle()
    {
        // Player is outside the scratch, walk, and chase range
        SetWalkingAnimation(true);
        SetRunningAnimation(false);
        MoveTowardsPlayer(walkingSpeed);
        StopMeleeAttack();
        EnableNavMeshAgent();
    }

    void StartMeleeAttack()
    {
        if (!isMeleeAttacking && !animator.GetBool("Melee"))
        {
            isMeleeAttacking = true;
            animator.SetBool("Melee", true);
            stopMelee = false; // Reset the flag
        }
    }

    void StopMeleeAttack()
    {
        if (isMeleeAttacking && animator.GetBool("Melee"))
        {
            isMeleeAttacking = false;
            animator.SetBool("Melee", false);
        }
    }

    void SetWalkingAnimation(bool value)
    {
        animator.SetBool("IsWalking", value);
    }

    void SetRunningAnimation(bool value)
    {
        animator.SetBool("IsRunning", value);
    }

    void MoveTowardsPlayer(float speed)
    {
        if (isScratching || isMeleeAttacking)
        {
            // Stop moving towards the player when in scratch range or during Melee attack
            return;
        }

        // Set destination to the player's position
        navMeshAgent.SetDestination(player.position);
    }

    void EnableNavMeshAgent()
    {
        if (!navMeshAgent.enabled)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.isStopped = false;
        }
    }

    void DisableNavMeshAgent()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }
    }

    // LateUpdate is used to ensure that this runs after the Animator's internal updates
    void LateUpdate()
    {
        if (stopMelee)
        {
            StopMeleeAttack();
            stopMelee = false;
        }
    }
}
