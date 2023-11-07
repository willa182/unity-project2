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

    public GameObject bloodSplashPrefab; // The blood splash particle system prefab
    private bool isBloodSplashActive = false;
    private bool isDead = false; // Flag to track if the enemy is dead

    private AmmoDrop ammoDrop;
    SoundManager soundManager;

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

        ammoDrop = GetComponent<AmmoDrop>();
        soundManager = SoundManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead && other.CompareTag("Bullet"))
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

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true; // Mark the enemy as dead
            animator.SetTrigger("Death");
            soundManager.PlayZombieDeath();
            healthBar.gameObject.SetActive(false);
            navMeshAgent.isStopped = true;

            if (isBloodSplashActive)
            {
                // Disable the blood splash effect
                bloodSplashPrefab.SetActive(false);
                isBloodSplashActive = false;
            }
        }
        else
        {
            healthBar.value = currentHealth;

            // Check if the health is below 25% and the scream trigger is not activated
            if (currentHealth <= health * 0.25f && !screamTriggerActivated)
            {
                // Set the Scream Trigger active
                animator.SetTrigger("IsScreaming");
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
        // Generate a random chance every time the enemy is damaged
        int randomChance = Random.Range(0, 100);

        // Check for IsStumbling and set the flag.
        if (randomChance < 25)
        {
            animator.SetTrigger("IsTakingHit");

            if (Random.Range(0, 100) < 10)
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

            // Generate a random chance for the blood splash
            int bloodSplashChance = Random.Range(0, 100);

            // If the chance is less than 50 (50% chance), play the blood splash
            if (bloodSplashChance < 50)
            {
                // Instantiate the blood splash prefab at the enemy's position
                GameObject bloodSplash = Instantiate(bloodSplashPrefab, transform.position, Quaternion.identity);
                bloodSplash.SetActive(true);
            }
        }
    }

    public void TakeExplosionDamage(int damageAmount)
    {
        if (!isDead)
        {
            currentHealth -= damageAmount;
            healthBar.gameObject.SetActive(true);
            healthBar.maxValue = health;

            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetTrigger("Death");
                healthBar.gameObject.SetActive(false);
                navMeshAgent.isStopped = true;
            }
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
