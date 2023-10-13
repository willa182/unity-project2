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

    Animator animator;

    public Image flashCanvas;
    public Slider healthSlider;

    private float flashCounter;
    private bool isDead = false; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        mousemovement = GetComponent<PlayerLook>();
        playerMotor = GetComponent<PlayerMotor>();
        gunFire = GetComponent<GunFires>();
        playerInventory = GetComponent<PlayerInventory>();
        currentHealth = startingHealth;
        flashCanvas.enabled = false;
        animator = GetComponent<Animator>();

        UpdateHealthBar();
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
            }
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
