using System.Collections;
using UnityEngine;
using static PlayerInventory;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public Sprite sprite;
    private bool canBePickedUp = false;

    public bool IsPickedUp { get; set; } = false;
    public WeaponTransformSettings transformSettings;

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
            canBePickedUp = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canBePickedUp = false;
        }
    }

    void PickUp()
    {
        PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            IsPickedUp = true;
            playerInventory.AddWeapon(this);

            StartCoroutine(DestroyWeaponDelayed());
        }
    }

    IEnumerator DestroyWeaponDelayed()
    {
        yield return new WaitForSeconds(2.0f); 
        Destroy(gameObject); 
    }
}
