using UnityEngine;

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
    private EnemyHealthManager healthManager;
    private PlayerHealthManager playerHealth;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        healthManager = GetComponent<EnemyHealthManager>();
        playerHealth = player.GetComponent<PlayerHealthManager>();
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            isChasing = false;
            SetWalkingAnimation(false);
            SetRunningAnimation(false);
            animator.SetBool("IsIdle", true);
            StopMeleeAttack();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (healthManager != null && healthManager.IsFlashing())
        {
            MoveTowardsPlayer(runningSpeed);
            StopMeleeAttack();
            return;
        }

        isScratching = (distanceToPlayer <= scratchRange);

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;

            if (isScratching)
            {
                // Player is within the scratch range
                SetWalkingAnimation(false);
                SetRunningAnimation(false);
                StartMeleeAttack();
            }
            else if (distanceToPlayer <= walkRange)
            {
                // Player is within the walk range
                SetWalkingAnimation(false);
                SetRunningAnimation(true);
                MoveTowardsPlayer(runningSpeed);
                StopMeleeAttack();
            }
            else
            {
                // Player is outside the scratch and walk range
                SetWalkingAnimation(true);
                SetRunningAnimation(false);
                MoveTowardsPlayer(walkingSpeed);
                StopMeleeAttack();
            }
        }
        else
        {
            isChasing = false;
            SetWalkingAnimation(false);
            SetRunningAnimation(false);
            animator.SetBool("IsIdle", true);
            StopMeleeAttack();
        }

        if (!isChasing)
        {
            // Handle non-chasing behavior maybe in the future
        }
    }

    void StartMeleeAttack()
    {
        if (!animator.GetBool("Melee"))
        {
            animator.SetBool("Melee", true);
        }
    }

    void StopMeleeAttack()
    {
        if (animator.GetBool("Melee") && !isScratching)
        {
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
        if (isScratching)
        {
            // Stop moving towards the player when in scratch range
            return;
        }
            Vector3 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
}
