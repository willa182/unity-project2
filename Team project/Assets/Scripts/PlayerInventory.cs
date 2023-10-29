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

    public Text grenadeCountText; // Reference to your UI Text for displaying grenade count.
    public int grenadeCount = 0;
    public PlayerMotor playerMotor;

    public GunFires gunFires;

    void Start()
    {
        InitializeQuickslots();
        Controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        UpdateGrenadeCountText();
        gunFires = GetComponent<GunFires>();
        PlayerMotor.OnGrenadeThrown += UpdateGrenadeCountText;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        PlayerMotor.OnGrenadeThrown -= UpdateGrenadeCountText;
    }


    void Update()
    {
        if (Input.GetKeyDown(pickupKey) && canPickup)
        {
            TryPickupWeaponLogic();
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

    private void PickupGrenade()
    {
        if (grenadeCount < 3) // Limit the player to a maximum of 3 grenades.
        {
            grenadeCount++;
            UpdateGrenadeCountText(); // Update the UI text with the new grenade count.

            // Call a method in the PlayerMotor script to update availableGrenades
            if (playerMotor != null)
            {
                playerMotor.UpdateAvailableGrenades(grenadeCount);
            }
        }
        else
        {
            Debug.Log("Grenade inventory is full!");
        }
    }

    public void DecrementGrenadeCount()
    {
        grenadeCount--;
        UpdateGrenadeCountText();
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

                if (gunFires != null)
                {
                    gunFires.UpdateAmmoText();
                }

                // Stop the pickup animation
                animator.SetBool("PickingUp", false);
                isPickupAnimationPlaying = false;
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

    void TryPickupWeaponLogic()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
        bool canPickup = false;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Grenade")) // Use the tag to identify grenades.
            {
                canPickup = true;
                break;
            }
            else
            {
                canPickup = true;
                break;
            }
        }

        if (canPickup && !isPickupAnimationPlaying)
        {
            // Find the Weapon component on the pickup object
            Weapon weaponPrefab = null;
            GameObject weaponObject = null;

            foreach (Collider collider in colliders)
            {
                Weapon weaponComponent = collider.GetComponent<Weapon>();
                if (weaponComponent != null)
                {
                    weaponPrefab = weaponComponent;
                    weaponObject = collider.gameObject;
                    break;
                }
            }

            if (weaponPrefab != null && weaponObject != null)
            {
                // Start the pickup animation
                animator.SetBool("PickingUp", true);
                isPickupAnimationPlaying = true;

                if (weaponPrefab.CompareTag("Grenade")) // Check the tag to identify grenades.
                {
                    PickupGrenade();
                }
                else
                {
                    // Handle other weapon pickups as before.
                    PickupWeapon(weaponPrefab, weaponObject);
                }

                // Start a coroutine to check if the weapon object is destroyed
                StartCoroutine(CheckWeaponDestroyed(weaponObject));
            }
        }
    }

    public void UpdateGrenadeCountText()
    {
        if (grenadeCountText != null)
        {
            grenadeCountText.text = "Grenades: " + grenadeCount;
            Debug.Log("UpdateGrenadeCountText: " + grenadeCountText.text);
        }
    }

    private IEnumerator CheckWeaponDestroyed(GameObject weaponObject)
    {
        while (true)
        {
            // Check if the weapon object has been destroyed
            if (weaponObject == null)
            {
                // The weapon object has been destroyed, stop the animation
                animator.SetBool("PickingUp", false);
                isPickupAnimationPlaying = false;
                yield break;
            }
            yield return null;
        }
    }

    public IEnumerator PickupProcess()
    {
        // Start the pickup animation
        animator.SetBool("PickingUp", true);
        isPickupAnimationPlaying = true;

        yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds

        // Handle the actual weapon pickup logic here
        TryPickupWeaponLogic();

        // Disable the pickup animation
        animator.SetBool("PickingUp", false);
        isPickupAnimationPlaying = false;
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

        yield return new WaitForSeconds(3f);

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

    private void PickupWeapon(Weapon weaponPrefab, GameObject weaponObject)
    {
        canPickup = false;

        AddWeapon(weaponPrefab);

        Destroy(weaponObject, 0.8f);

        canPickup = true;
    }

    public void AddWeapon(Weapon weapon)
    {
        bool weaponAlreadyInInventory = weaponsInventory.Contains(weapon);

        if (!weaponAlreadyInInventory)
        {
            if (!weapon.CompareTag("Grenade")) // Skip adding grenades to quickslots.
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
            }

            UpdateQuickslotUI();
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

