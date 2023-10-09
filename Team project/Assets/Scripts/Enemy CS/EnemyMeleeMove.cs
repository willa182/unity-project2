using UnityEngine;

public class EnemyMeleeMove : MonoBehaviour
{
    public float chaseRange = 15f;
    public float WalkRange = 10f;
    public float walkingSpeed = 3f;
    public float runningSpeed = 6f;

    private Transform player;
    private Animator animator;
    private bool isChasing = false;
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
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (healthManager != null && healthManager.IsFlashing())
        {
            MoveTowardsPlayer(runningSpeed);
            return;
        }

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;

            if (distanceToPlayer <= WalkRange)
            {
                SetWalkingAnimation(false);
                SetRunningAnimation(true);
                MoveTowardsPlayer(runningSpeed);
            }
            else
            {
                SetWalkingAnimation(true);
                SetRunningAnimation(false);
                MoveTowardsPlayer(walkingSpeed);
            }
        }
        else
        {
            isChasing = false;
            SetWalkingAnimation(false);
            SetRunningAnimation(false);

            animator.SetBool("IsIdle", true);
        }

        if (!isChasing)
        {
            // Handle non-chasing behavior maybe in future
        }
    }

    void ChasePlayer()
    {
        // Implement chasing behavior maybe in future
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
        Vector3 direction = (player.position - transform.position).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }
}
