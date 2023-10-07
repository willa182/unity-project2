using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    // Store weapon prefab names here
    public List<string> weaponPrefabNames = new List<string>();

    // The actual weapon prefabs loaded from Resources
    private List<Weapon> weaponsInventory = new List<Weapon>();

    // Store sprites for the quickslots
    private List<Sprite> quickslotSprites = new List<Sprite>();

    public Transform quickslotUI;
    public Transform handTransform; // Reference to the hand transform where weapons will be instantiated
    private int equippedSlotIndex = -1;

    // Reference the slots in the inspector
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;

    public KeyCode pickupKey = KeyCode.E;
    public float pickupDelay = 1f; 
    private bool canPickup = true; 
    private GameObject equippedWeaponInstance;

    void Start()
    {
        slot1 = quickslotUI.Find("Slot1").gameObject;
        slot2 = quickslotUI.Find("Slot2").gameObject;
        slot3 = quickslotUI.Find("Slot3").gameObject;

 
        foreach (string weaponPrefabName in weaponPrefabNames)
        {
            Weapon weaponPrefab = Resources.Load<Weapon>("Weapons/" + weaponPrefabName);
            if (weaponPrefab != null)
            {
                weaponsInventory.Add(weaponPrefab);
            }
            else
            {
                Debug.LogError("Failed to load weapon prefab: " + weaponPrefabName);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupWeapon();
        }

       
        for (int i = 1; i <= 3; i++)
        {
            KeyCode key = KeyCode.Alpha0 + i;
            if (Input.GetKeyDown(key))
            {
                TryEquipWeapon(i);
            }
        }
    }

    void TryEquipWeapon(int slotIndex)
    {
        if (slotIndex > 0 && slotIndex <= weaponsInventory.Count)
        {
            Weapon selectedWeapon = weaponsInventory[slotIndex - 1]; // Adjust the index

            if (selectedWeapon != null)
            {
                if (selectedWeapon.IsPickedUp)
                {
                    Debug.Log("Trying to equip weapon: " + selectedWeapon.weaponName);
                    EquipWeapon(slotIndex);
                }
                else
                {
                    Debug.LogError("Cannot equip. Weapon is not picked up. IsPickedUp: " + selectedWeapon.IsPickedUp);
                }
            }
            else
            {
                Debug.LogError("Cannot equip. Selected weapon is null.");
            }
        }
        else
        {
            Debug.LogError("Invalid slot index.");
        }
    }


    void TryPickupWeapon()
    {
        // Check if the player pressed the pickup key
        if (Input.GetKeyDown(pickupKey))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
            Debug.Log("Number of colliders: " + colliders.Length);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("WeaponPickup"))
                {
                    Weapon weaponPrefab = collider.GetComponent<Weapon>();
                    if (weaponPrefab != null)
                    {
                        if (weaponsInventory.Count < 3 && !weaponsInventory.Contains(weaponPrefab))
                        {
                            StartCoroutine(PickupWithDelay(weaponPrefab, collider.gameObject));
                            return;
                        }
                        else
                        {
                            Debug.Log("Quickslots are full or weapon is already in inventory!");
                            return;
                        }
                    }
                }
            }
        }
    }

    public void HandleWeaponPickup(Weapon weapon)
    {
        Debug.Log("HandleWeaponPickup called");
        if (Input.GetKeyDown(pickupKey))
        {
            Debug.Log("E is pressed");
            AddWeapon(weapon);

        
            weapon.IsPickedUp = true;
            Debug.Log("Weapon picked up: " + weapon.gameObject.name);

          
            Renderer renderer = weapon.GetComponent<Renderer>();
            Collider collider = weapon.GetComponent<Collider>();

            if (renderer != null)
                renderer.enabled = false;

            if (collider != null)
                collider.enabled = false;

      
            Destroy(weapon.gameObject, 0.1f);

         
            TryEquipWeapon(weaponsInventory.IndexOf(weapon) + 1); 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
            if (playerInventory != null)
            {
            
                Weapon weaponComponent = GetComponent<Weapon>();
                if (weaponComponent != null)
                {
                    playerInventory.HandleWeaponPickup(weaponComponent);
                }
            }
        }
    }


    IEnumerator PickupWithDelay(Weapon weaponPrefab, GameObject weaponObject)
    {
        canPickup = false; 
        yield return new WaitForSeconds(pickupDelay);

        AddWeapon(weaponPrefab); 
        Destroy(weaponObject); 

        canPickup = true; 
    }

    public void AddWeapon(Weapon weapon)
    {
    
        if (weaponsInventory.Count < 3)
        {
            weaponsInventory.Add(weapon);

       
            weapon.IsPickedUp = true;

            UpdateQuickslotUI();
        }
        else
        {
            Debug.Log("Quickslots are full!");
     
        }
    }

    public void RemoveWeapon(Weapon weapon)
    {
        weaponsInventory.Remove(weapon);
        UpdateQuickslotUI();
    }

    void UpdateQuickslotUI()
    {
    
        ClearSlots();

    
        for (int i = 0; i < weaponsInventory.Count; i++)
        {
            GameObject currentSlot = GetSlot(i);

         
            Image slotImage = currentSlot.GetComponent<Image>();
            if (slotImage != null && slotImage.sprite == null)
            {
            
                if (i == equippedSlotIndex)
                {
                  
                    slotImage.sprite = null;
                }
                else
                {
               
                    slotImage.sprite = weaponsInventory[i].sprite;
                }

                break; 
            }
        }
    }

    void ClearSlots()
    {

        foreach (Transform child in quickslotUI)
        {
            Image slotImage = child.GetComponent<Image>();
            if (slotImage != null)
            {
                slotImage.sprite = null; 
            }
        }

       
        quickslotSprites.Clear();
    }

    GameObject GetSlot(int index)
    {
      
        switch (index)
        {
            case 0:
                return slot1;
            case 1:
                return slot2;
            case 2:
                return slot3;
            default:
                return null;
        }
    }

    void EquipWeapon(int slotIndex)
    {
        if (slotIndex > 0 && slotIndex < weaponsInventory.Count)
        {
       
            Weapon selectedWeapon = weaponsInventory[slotIndex];

            if (selectedWeapon != null && selectedWeapon.IsPickedUp)
            {
                Debug.Log("Equipping weapon: " + selectedWeapon.weaponName);

               
                UnequipWeapon();

             
                equippedWeaponInstance = Instantiate(selectedWeapon.gameObject, handTransform);

             
                equippedWeaponInstance.transform.localPosition = Vector3.zero;
                equippedWeaponInstance.transform.localRotation = Quaternion.identity;

                equippedWeaponInstance.transform.localScale = Vector3.one;

                equippedWeaponInstance.transform.parent = handTransform;

            
                equippedSlotIndex = slotIndex;

             
                UpdateQuickslotUI();
            }
            else
            {
                if (selectedWeapon == null)
                {
                    Debug.LogError("Selected weapon is null.");
                }
                else
                {
                    Debug.LogError("Selected weapon has not been picked up yet. IsPickedUp: " + selectedWeapon.IsPickedUp);
                }
            }
        }
        else
        {
            Debug.LogError("Invalid slot index.");
        }
    }

    void UnequipWeapon()
    {
   
        if (equippedWeaponInstance != null)
        {
 
            Destroy(equippedWeaponInstance);

          
            equippedSlotIndex = -1;

         
            UpdateQuickslotUI();
        }
    }
}
