using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class WeaponTransformSettings
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
    }

    [System.Serializable]
    public class WeaponPrefabInfo
    {
        public string weaponPrefabName;
        public WeaponTransformSettings transformSettings;
    }

    public List<WeaponPrefabInfo> weaponPrefabInfos = new List<WeaponPrefabInfo>();
    private List<Weapon> weaponsInventory = new List<Weapon>();
    private List<GameObject> quickslotSlots = new List<GameObject>();

    public Transform quickslotUI;
    public Transform handTransform;
    private int equippedSlotIndex = -1;
    private int selectedWeaponIndex = -1;

    public CharacterController Controller;
    public Animator animator;
    public KeyCode pickupKey = KeyCode.E;
    private bool canPickup = true;
    private GameObject equippedWeaponInstance;
    private bool isPickupAnimationPlaying = false;
    private bool IsIdle;

    void Start()
    {
        InitializeQuickslots();
        Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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

    public bool CanPickUp()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("WeaponPickup"))
            {
                Weapon weaponPrefab = collider.GetComponent<Weapon>();

                if (weaponPrefab != null && !weaponsInventory.Contains(weaponPrefab))
                {
                    return true;
                }
            }
        }

        return false;
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
        // Check if there's an object with the desired layer tag nearby
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        bool canPickup = false;

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("WeaponPickup"))
            {
                canPickup = true;
                break; // No need to check further once we find a valid object
            }
        }

        if (canPickup && !isPickupAnimationPlaying && canPickup)
        {
            Debug.Log("PickingUp animation triggered.");
            animator.SetBool("PickingUp", true);
            isPickupAnimationPlaying = true;

            StartCoroutine(ResetPickupFlag());
        }
    }

    public IEnumerator ResetPickupFlag()
    {
        // Assuming InputManager script is attached to the same GameObject
        InputManager inputManager = GetComponent<InputManager>();

        if (inputManager != null)
        {
            inputManager.enabled = false;
        }

        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        yield return new WaitForSeconds(2.5f);

        animator.SetBool("PickingUp", false);
        isPickupAnimationPlaying = false;

        // Inform the animation system that the pickup animation is complete
        animator.SetTrigger("PickedUp");

        // Re-enable InputManager script
        if (inputManager != null)
        {
            inputManager.enabled = true;
        }

        Controller.enabled = true;
        IsIdle = true;

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    void PickupWeapon(Weapon weaponPrefab, GameObject weaponObject)
    {
        canPickup = false;

        AddWeapon(weaponPrefab);

        Destroy(weaponObject, 1f);

        canPickup = true;
    }

    public void AddWeapon(Weapon weapon)
    {
        int emptySlotIndex = weaponsInventory.FindIndex(w => !w.IsPickedUp);

        if (emptySlotIndex != -1)
        {
            weaponsInventory[emptySlotIndex] = weapon;
        }
        else if (weaponsInventory.Count < 3)
        {
            weaponsInventory.Add(weapon);
        }
        else
        {
            Debug.Log("Quickslots are full!");
            return;
        }

        UpdateQuickslotUI();
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

                WeaponTransformSettings transformSettings = selectedWeapon.transformSettings;

                equippedWeaponInstance.transform.localPosition = transformSettings.position;
                equippedWeaponInstance.transform.localRotation = Quaternion.Euler(transformSettings.rotation.eulerAngles);
                equippedWeaponInstance.transform.localScale = transformSettings.scale;

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

    public void HandleWeaponPickup(Weapon weapon)
    {
        AddWeapon(weapon);
        DestroyImmediate(weapon.gameObject);
        UpdateQuickslotUI();
    }
}
