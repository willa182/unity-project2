using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    public float flashLength;
    public Color flashColor;

    private CharacterController characterController;
    private PlayerLook mousemovement;
    private PlayerMotor playerMotor;
    private GunFires gunFire;
    private PlayerInventory playerInventory;
    private InputManager inputManager;

    Animator animator;
    public int lowHealthThreshold = 25;
    private bool hasPlayedLowHealthSound = false;

    public Image flashCanvas;
    public Slider healthSlider;

    public int healthRestoreAmountPrefab1;
    public int healthRestoreAmountPrefab2;

    private float flashCounter;
    private bool isDead = false; 

    private SoundManager soundManager;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mousemovement = GetComponent<PlayerLook>();
        playerMotor = GetComponent<PlayerMotor>();
        gunFire = GetComponent<GunFires>();
        playerInventory = GetComponent<PlayerInventory>();
        inputManager = GetComponent<InputManager>();
        currentHealth = startingHealth;
        flashCanvas.enabled = false;
        animator = GetComponent<Animator>();
        soundManager = SoundManager.instance;
        UpdateHealthBar();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player runs over Prefab 1
        if (other.CompareTag("HealthPrefab1"))
        {
            // Restore health based on the amount for Prefab 1
            RestoreHealth(healthRestoreAmountPrefab1);

            // Optionally, you can disable or destroy the prefab after the player runs over it
            other.gameObject.SetActive(false);  // or Destroy(other.gameObject);
        }
        // Check if the player runs over Prefab 2
        else if (other.CompareTag("HealthPrefab2"))
        {
            // Restore health based on the amount for Prefab 2
            RestoreHealth(healthRestoreAmountPrefab2);

            // Optionally, you can disable or destroy the prefab after the player runs over it
            other.gameObject.SetActive(false);  // or Destroy(other.gameObject);
        }
    }

    private void RestoreHealth(int amount)
    {
        if (!isDead)
        {
            currentHealth += amount;

            // Ensure health does not exceed starting health
            currentHealth = Mathf.Min(currentHealth, startingHealth);

            UpdateHealthBar();
        }
    }


    void Update()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            animator.SetTrigger("Death");

            if (characterController != null)
            {
                characterController.enabled = false;
                mousemovement.enabled = false;
                playerInventory.enabled = false;
                playerMotor.enabled = false;
                gunFire.enabled = false;
                inputManager.enabled = false;
                soundManager.StopHeartBeatSound();
            }
        }

        // Check for low health and play heartbeat sound once
        if (currentHealth <= lowHealthThreshold && !hasPlayedLowHealthSound)
        {
            soundManager.PlayHeartBeatSound();
            hasPlayedLowHealthSound = true; // Set the flag to prevent looping
        }

        if (flashCounter > 0)
        {
            flashCounter -= Time.deltaTime;

            if (flashCounter <= 0)
            {
                flashCanvas.enabled = false;
            }
        }
    }

    public void HurtPlayer(int damageAmount)
    {
        if (!isDead) 
        {
            currentHealth -= damageAmount;
            flashCounter = flashLength;
            flashCanvas.enabled = true;

            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                isDead = true;
                animator.SetTrigger("Death");

                if (characterController != null)
                {
                    characterController.enabled = false;
                    mousemovement.enabled = false;
                    playerInventory.enabled = false;
                    playerMotor.enabled = false;
                    gunFire.enabled = false;
                    inputManager.enabled = false;
                }
            }
        }
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            float healthPercent = (float)currentHealth / startingHealth;
            healthSlider.value = healthPercent;
        }
    }
}
