using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public int startingHealth;
    public int currentHealth;
    public float flashLength;
    public Color flashColor;

    public Image flashCanvas;
    public Slider healthSlider; 

    private float flashCounter;

    void Start()
    {
        currentHealth = startingHealth;
        flashCanvas.enabled = false;

 
        UpdateHealthBar();
    }

    void Update()
    {
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
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
        currentHealth -= damageAmount;
        flashCounter = flashLength;
        flashCanvas.enabled = true;

    
        UpdateHealthBar();
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
