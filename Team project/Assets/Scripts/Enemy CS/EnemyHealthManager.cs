using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    public int health;
    private int currentHealth;

    public float flashLength;
    private float flashCounter;

    private Renderer rend;
    private Color storedColor;

    public Slider healthBar; // Reference to the health bar/slider UI
    private RectTransform sliderRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        rend = GetComponent<Renderer>();
        storedColor = rend.material.GetColor("_Color");

        // Initially set the health bar as inactive
        healthBar.gameObject.SetActive(false);

        // Get the RectTransform component from the health bar's slider
        sliderRectTransform = healthBar.GetComponent<RectTransform>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            int damageAmount = 1;
            HurtEnemy(damageAmount);

            // Activate the health bar when the enemy takes damage
            healthBar.gameObject.SetActive(true);

            // Set the maxValue of the slider when it becomes active
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

        // Check if the enemy still exists
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            // Disable the health slider when the enemy is destroyed
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            // Update the health bar value
            healthBar.value = currentHealth;

            // Ensure the health bar stays above the enemy's head
            Vector3 worldPos = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            sliderRectTransform.position = screenPoint;
        }
    }

    public void HurtEnemy(int damageAmount)
    {
        if (flashCounter <= 0)
        {
            currentHealth -= damageAmount;
            flashCounter = flashLength;
            rend.material.SetColor("_Color", Color.black);
        }
    }
}
