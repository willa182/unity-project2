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

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Ensure that NavMeshAgent is attached
        if (!navMeshAgent)
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= firingRange)
        {
            // Player is within firing range, stop moving and start shooting
            isChasing = false;
            SetRunningAnimation(false);
            SetShootingAnimation(true);
            StopMovement();
            // Add logic to handle shooting behavior here

            // For example, you might use a coroutine to simulate shooting duration
            StartCoroutine(StopShootingAfterDuration());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            // Player is within chase range but outside firing range
            isChasing = true;
            SetRunningAnimation(true);
            SetShootingAnimation(false);
            MoveTowardsPlayer(runningSpeed);
        }
        else
        {
            // Player is outside chase range
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
        // Wait for the shooting duration and then stop shooting
        yield return new WaitForSeconds(shootingDuration);

        // Add logic to stop shooting here
        SetShootingAnimation(false);
        // You might add additional logic here, such as transitioning to idle or moving towards the player again
    }

    // Helper methods to set animator parameters
    void SetRunningAnimation(bool value)
    {
        animator.SetBool("IsRunning", value);
    }

    void SetShootingAnimation(bool value)
    {
        animator.SetBool("IsShooting", value);
    }
}
