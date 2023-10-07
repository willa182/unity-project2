using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public Sprite sprite;
    public bool IsPickedUp { get;set; } = false;


    private bool canBePickedUp = false;

    void Update()
    {
      
        if (canBePickedUp && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
          
                canBePickedUp = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
             
                canBePickedUp = false;
            }
        }
    }

    void PickUp()
    {
        Debug.Log("PickUp method called for weapon: " + weaponName);
      
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            playerInventory.HandleWeaponPickup(this);
        }
    }
}
