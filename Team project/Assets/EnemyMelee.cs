using System.Collections;
using UnityEngine;

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
    public LayerMask groundLayer;

    void Start()
    {
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

        animator.SetBool("IsWalking", true);
        float walkTime = Random.Range(3f, 6f);
        float startTime = Time.time;

        while (Time.time - startTime < walkTime)
        {
            // Calculate the movement direction.
            Vector3 moveDirection = (randomDirection - transform.position).normalized;

            // Move the enemy by updating its position directly.
            transform.position += moveDirection * walkSpeed * Time.deltaTime;

            // Ensure the enemy stays above the ground.
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }

            // Update the enemy's rotation to face the movement direction.
            if (moveDirection != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = rotation;
            }

            yield return null;
        }

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
                // If player is in chase range, set IsRunning animation
                animator.SetBool("IsRunning", true);
            }
            else
            {
                // Player is out of chase range
                animator.SetBool("IsRunning", false);
            }
        }

        if (!isScreaming && distanceToPlayer < attackRange)
        {
            // Enable melee attack if not screaming and within attack range
            animator.SetBool("Melee", true);
        }
        else
        {
            // Disable melee attack otherwise
            animator.SetBool("Melee", false);
        }
    }

    void FacePlayer()
    {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
}
