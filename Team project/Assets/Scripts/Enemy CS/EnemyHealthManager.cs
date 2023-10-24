using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    public int health;
    public int currentHealth;

    public float flashLength;
    private float flashCounter;

    private Renderer rend;
    private Color storedColor;

    public Slider healthBar;
    private RectTransform sliderRectTransform;

    private Transform player;
    private NavMeshAgent navMeshAgent;
    Animator animator;

    public float moveSpeed = 5f;
    private System.Random random = new System.Random();

    private EnemyMelee randomwalk;
    private bool screamTriggerActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        rend = GetComponent<Renderer>();
        storedColor = rend.material.GetColor("_Color");

        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        healthBar.gameObject.SetActive(false);


        sliderRectTransform = healthBar.GetComponent<RectTransform>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            int damageAmount = 1;
            HurtEnemy(damageAmount);


            healthBar.gameObject.SetActive(true);


            healthBar.maxValue = health;

            Destroy(other.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (flashCounter > 0)
        {
            flashCounter -= Time.deltaTime;
            if (flashCounter <= 0)
            {
                rend.material.SetColor("_Color", storedColor);
            }
        }


        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.value = currentHealth;

            // Check if the health is below 25% and the scream trigger is not activated
            if (currentHealth <= health * 0.25f && !screamTriggerActivated)
            {
                // Set the Scream Trigger active
                animator.SetTrigger("ScreamTrigger");
                screamTriggerActivated = true;
            }

            Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            sliderRectTransform.position = screenPoint;

            if (flashCounter > 0)
            {
                MoveTowardsPlayer();
            }
        }
    }

    public void HurtEnemy(int damageAmount)
    {
        int randomChance = random.Next(100);

        // Check for IsStumbling and set the flag.
        if (randomChance < 25)
        {
            animator.SetTrigger("IsTakingHit");

            if (random.Next(100) < 10)
            {
                animator.SetTrigger("IsStumbling");
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
                navMeshAgent.speed = 0f;
                randomwalk.isRandomWalkEnabled = false;
            }
        }

        if (flashCounter <= 0)
        {
            currentHealth -= damageAmount;
            flashCounter = flashLength;
            rend.material.SetColor("_Color", Color.black);
        }
    }


    public bool IsFlashing()
    {
        return flashCounter > 0;
    }

    void MoveTowardsPlayer()
    {
        if (navMeshAgent != null)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            Vector3 destination = transform.position + directionToPlayer;

            navMeshAgent.SetDestination(destination);
        }
        else
        {
            Debug.LogError("NavMeshAgent component is missing!");
        }
    }
}
