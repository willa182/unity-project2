using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyGunMove : MonoBehaviour
{
    public float chaseRange = 15f;
    public float firingRange = 10f;
    public float runningSpeed = 6f;
    public float shootingDuration = 2f;

    private Transform player;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    private PlayerHealthManager playerHealth; 

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerHealth = player.GetComponent<PlayerHealthManager>();

        if (!navMeshAgent)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }

    void Update()
    {
        if (playerHealth && playerHealth.currentHealth <= 0)
        {
            isChasing = false;
            SetRunningAnimation(false);
            SetShootingAnimation(false);
            StopMovement();
            animator.SetBool("IsIdle", true);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= firingRange)
        {
            isChasing = false;
            SetRunningAnimation(false);
            SetShootingAnimation(true);
            StopMovement();

            StartCoroutine(StopShootingAfterDuration());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
            SetRunningAnimation(true);
            SetShootingAnimation(false);
            MoveTowardsPlayer(runningSpeed);
        }
        else
        {
            isChasing = false;
            SetRunningAnimation(false);
            SetShootingAnimation(false);
            StopMovement();
            animator.SetBool("IsIdle", true);
        }
    }

    void MoveTowardsPlayer(float speed)
    {
        if (navMeshAgent)
        {
            navMeshAgent.SetDestination(player.position);
            navMeshAgent.speed = speed;
        }
    }

    void StopMovement()
    {
        if (navMeshAgent)
        {
            navMeshAgent.ResetPath();
        }
    }

    IEnumerator StopShootingAfterDuration()
    {
        yield return new WaitForSeconds(shootingDuration);

        SetShootingAnimation(false);
    }

    void SetRunningAnimation(bool value)
    {
        animator.SetBool("IsRunning", value);
    }

    void SetShootingAnimation(bool value)
    {
        animator.SetBool("IsShooting", value);
    }
}
