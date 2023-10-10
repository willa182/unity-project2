using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<string> weaponPrefabNames = new List<string>();
    private List<Weapon> weaponsInventory = new List<Weapon>();
    private List<GameObject> quickslotSlots = new List<GameObject>();

    public Transform quickslotUI;
    public Transform handTransform;
    private int equippedSlotIndex = -1;
    private int selectedWeaponIndex = -1;

    public KeyCode pickupKey = KeyCode.E;
    private bool canPickup = true;
    private GameObject equippedWeaponInstance;

    public Vector3 weaponPosition = new Vector3(-0.000542f, 0.001634f, -0.001138f);
    public Quaternion weaponRotation = Quaternion.Euler(-142.898f, 168.012f, 22.036f);
    public Vector3 weaponScale = new Vector3(0.0012f, 0.0012f, 0.0012f);

    void Start()
    {
        InitializeQuickslots();
        LoadWeaponPrefabs();
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey) && canPickup)
        {
            TryPickupWeapon();
        }

        for (int i = 1; i <= quickslotSlots.Count; i++)
        {
            KeyCode key = KeyCode.Alpha0 + i;
            if (Input.GetKeyDown(key))
            {
                TryEquipWeapon(i);
            }
        }

        if (equippedSlotIndex != -1)
        {
            InstantiateWeaponInHand(weaponsInventory[equippedSlotIndex]);
        }
    }

    void TryEquipWeapon(int slotIndex)
    {
        if (slotIndex > 0 && slotIndex <= weaponsInventory.Count)
        {
            selectedWeaponIndex = slotIndex;

            if (selectedWeaponIndex <= weaponsInventory.Count)
            {
                UnequipWeapon();
                InstantiateWeaponInHand(weaponsInventory[selectedWeaponIndex - 1]);
                UpdateQuickslotUI();
            }
            else
            {
                Debug.LogError("Selected weapon is not in the inventory!");
            }
        }
        else
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
        }
    }

    void TryPickupWeapon()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("WeaponPickup"))
            {
                Weapon weaponPrefab = collider.GetComponent<Weapon>();
                if (weaponPrefab != null && weaponsInventory.Count < 3 && !weaponsInventory.Contains(weaponPrefab))
                {
                    AddWeapon(weaponPrefab);

                    canPickup = true;
                    return;
                }
            }
        }
        canPickup = true;
    }

    void PickupWeapon(Weapon weaponPrefab, GameObject weaponObject)
    {
        canPickup = false;

        AddWeapon(weaponPrefab);

        DestroyImmediate(weaponObject);

        canPickup = true;
    }

    void AddWeapon(Weapon weapon)
    {
        if (weaponsInventory.Count < 3)
        {
            int emptySlotIndex = -1;
            for (int i = 0; i < weaponsInventory.Count; i++)
            {
                if (!weaponsInventory[i].IsPickedUp)
                {
                    emptySlotIndex = i;
                    break;
                }
            }

            if (emptySlotIndex == -1)
            {
                emptySlotIndex = weaponsInventory.Count;
            }

            weaponsInventory[emptySlotIndex] = weapon;

            UpdateQuickslotUI();
        }
        else
        {
            Debug.Log("Quickslots are full!");
        }
    }

    void InstantiateWeaponInHand(Weapon selectedWeapon)
    {
        UnequipWeapon();

        if (selectedWeapon.IsPickedUp)
        {
            GameObject weaponPrefab = Resources.Load<GameObject>("Weapons/" + selectedWeapon.weaponName);

            if (weaponPrefab != null)
            {
                equippedWeaponInstance = Instantiate(weaponPrefab, handTransform);
                equippedWeaponInstance.transform.SetParent(handTransform);
                equippedWeaponInstance.transform.localPosition = weaponPosition;
                equippedWeaponInstance.transform.localRotation = weaponRotation;
                equippedWeaponInstance.transform.localScale = weaponScale;

                UpdateQuickslotUI();
            }
            else
            {
                Debug.LogError("Failed to load weapon prefab: " + selectedWeapon.weaponName);
            }
        }
        else
        {
            Debug.LogWarning("Attempted to instantiate a weapon that has not been picked up.");
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

    void UpdateQuickslotUI()
    {
        for (int i = 0; i < quickslotSlots.Count; i++)
        {
            Image slotImage = quickslotSlots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                if (i == equippedSlotIndex)
                {
                    slotImage.sprite = null;
                }
                else if (i < weaponsInventory.Count && weaponsInventory[i].IsPickedUp)
                {
                    slotImage.sprite = weaponsInventory[i].sprite;
                }
                else
                {
                    slotImage.sprite = null;
                }
            }
        }
    }

    void InitializeQuickslots()
    {
        foreach (Transform child in quickslotUI)
        {
            quickslotSlots.Add(child.gameObject);
        }
    }

    void LoadWeaponPrefabs()
    {
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

    public void HandleWeaponPickup(Weapon weapon)
    {
        AddWeapon(weapon);
        DestroyImmediate(weapon.gameObject);
        UpdateQuickslotUI();
    }
}
