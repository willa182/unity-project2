using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    public float flashLength;
    public Color flashColor;

    Animator animator;

    public Image flashCanvas;
    public Slider healthSlider;

    private float flashCounter;
    private bool isDead = false; 

    void Start()
    {
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
