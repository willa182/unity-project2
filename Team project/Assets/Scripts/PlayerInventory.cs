using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    
    public List<string> weaponPrefabNames = new List<string>();

  
    private List<Weapon> weaponsInventory = new List<Weapon>();

   
    private List<Sprite> quickslotSprites = new List<Sprite>();

    public Transform quickslotUI;
    public Transform handTransform; 
    private int equippedSlotIndex = -1;
    private int selectedWeaponIndex = -1;


    
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;

    public KeyCode pickupKey = KeyCode.E;
    public float pickupDelay = 1f; 
    private bool canPickup = true; 
    private GameObject equippedWeaponInstance;
    private GameObject pickedUpWeaponInstance;

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

     
        if (equippedSlotIndex == selectedWeaponIndex && selectedWeaponIndex != -1)
        {
            InstantiateWeaponInHand(weaponsInventory[selectedWeaponIndex - 1]);
        }
    }

    void TryEquipWeapon(int slotIndex)
    {
        Debug.Log("TryEquipWeapon called for slot: " + slotIndex);
        if (slotIndex > 0 && slotIndex <= weaponsInventory.Count)
        {
            selectedWeaponIndex = slotIndex;
            Debug.Log("Selected weapon: " + weaponsInventory[selectedWeaponIndex - 1].weaponName);
            TryEquipPickedUpWeapon();
        }
        else
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
        }
    }

    public void TryEquipPickedUpWeapon()
    {
        if (pickedUpWeaponInstance != null)
        {
        
            Destroy(pickedUpWeaponInstance);        
            
                Weapon selectedWeapon = weaponsInventory[selectedWeaponIndex - 1];
                GameObject weaponPrefab = Resources.Load<GameObject>("Weapons/" + selectedWeapon.weaponName);
                pickedUpWeaponInstance = Instantiate(weaponPrefab, handTransform);
               
            
                Debug.LogError("Error instantiating weapon: ");
            
       
            pickedUpWeaponInstance.transform.localPosition = Vector3.zero;
            pickedUpWeaponInstance.transform.localRotation = Quaternion.identity;
            pickedUpWeaponInstance.transform.localScale = Vector3.one;

           
            pickedUpWeaponInstance.transform.parent = handTransform;

      
            equippedSlotIndex = selectedWeaponIndex;

        
            UpdateQuickslotUI();
        }
    }


    void InstantiateWeaponInHand(Weapon selectedWeapon)
    {
        Debug.Log("InstantiateWeaponInHand called for weapon: " + selectedWeapon.weaponName);
      
        UnequipWeapon();

     
        GameObject weaponPrefab = Resources.Load<GameObject>("Weapons/" + selectedWeapon.weaponName);

        if (weaponPrefab != null)
        {
        
            equippedWeaponInstance = Instantiate(weaponPrefab, handTransform);

        
            equippedWeaponInstance.transform.localPosition = new Vector3(-0.000542f, 0.001634f, -0.001138f);
            equippedWeaponInstance.transform.localRotation = Quaternion.Euler(-142.898f, 168.012f, 22.036f);

          
            equippedWeaponInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

         
            equippedWeaponInstance.transform.parent = handTransform;

        
            equippedSlotIndex = selectedWeaponIndex; 

        
            UpdateQuickslotUI();
        }
        else
        {
            Debug.LogError("Failed to load weapon prefab: " + selectedWeapon.weaponName);
        }
    }


    void TryPickupWeapon()
    {
       
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

        
            Renderer renderer = weapon.GetComponent<Renderer>();
            Collider collider = weapon.GetComponent<Collider>();

            if (renderer != null)
                renderer.enabled = false;

            if (collider != null)
                collider.enabled = false;

      
            Destroy(weapon.gameObject, 1f);

      
            UpdateQuickslotUI();

    
            selectedWeaponIndex = weaponsInventory.Count;

         
            TryEquipPickedUpWeapon();
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

      
            selectedWeaponIndex = weaponsInventory.Count;

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

    void EquipWeapon(Weapon selectedWeapon)
    {
    
        UnequipWeapon();

      
        GameObject weaponPrefab = Resources.Load<GameObject>("Weapons/" + selectedWeapon.weaponName);

        if (weaponPrefab != null)
        {
       
            equippedWeaponInstance = Instantiate(weaponPrefab, handTransform);

       
            equippedWeaponInstance.transform.localPosition = new Vector3(-0.000542f, 0.001634f, -0.001138f);
            equippedWeaponInstance.transform.localRotation = Quaternion.Euler(-142.898f, 168.012f, 22.036f);
            equippedWeaponInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

    
            equippedWeaponInstance.transform.parent = handTransform;

       
            selectedWeapon.IsPickedUp = true;

     
            equippedSlotIndex = selectedWeaponIndex;

         
            UpdateQuickslotUI();
        }
        else
        {
            Debug.LogError("Failed to load weapon prefab: " + selectedWeapon.weaponName);
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
