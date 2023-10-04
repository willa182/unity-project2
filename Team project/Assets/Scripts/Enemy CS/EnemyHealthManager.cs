using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    public int health;
    private int currentHealth;

    public float flashLength;
    private float flashCounter;

    private Renderer rend;
    private Color storedColor;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = health;
        rend = GetComponent<Renderer>();
        storedColor = rend.material.GetColor("_Color");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet")) 
        {
            int damageAmount = 1; 
            HurtEnemy(damageAmount);
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
