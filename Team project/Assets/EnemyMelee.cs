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

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Transform playerTransform;
    private bool isScreaming = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

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

        navMeshAgent.SetDestination(hit.position);

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

            // If player is in chase range, set IsRunning animation and run towards the player
            if (distanceToPlayer < chaseRange)
            {
                animator.SetBool("IsRunning", true);
                navMeshAgent.speed = runSpeed;
                navMeshAgent.SetDestination(playerTransform.position);
            }
            else
            {
                animator.SetBool("IsRunning", false);
                navMeshAgent.speed = walkSpeed;
            }
        }

        if (isScreaming || distanceToPlayer < chaseRange)
        {
            // Disable other movements during scream or when in chase range
            navMeshAgent.isStopped = true;
            animator.SetBool("Melee", false);
        }
        else
        {
            // Enable movements when not screaming and not in chase range
            navMeshAgent.isStopped = false;

            if (distanceToPlayer < attackRange)
            {
                animator.SetBool("Melee", true);
            }
            else
            {
                animator.SetBool("Melee", false);
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
