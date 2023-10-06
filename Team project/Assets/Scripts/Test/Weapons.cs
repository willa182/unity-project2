using UnityEngine;

public class Weapons : MonoBehaviour
{
    public string weaponName = "Default Weapon"; // Add this line
    public float damage = 5f;

    public WeaponSystem weaponSystem;

    private void Start()
    {
        // Search for the WeaponSystem component on the parent GameObject
        weaponSystem = transform.parent.GetComponent<WeaponSystem>();

        // Make sure weaponSystem is not null.
        if (weaponSystem != null)
        {
            UpdateWeaponProperties(); // Call this initially to set the weapon's properties.
        }
        else
        {
            Debug.LogWarning("WeaponSystem reference not found on the parent GameObject.");
        }
    }


    // Function to update the weapon's properties based on the currently equipped weapon
    private void UpdateWeaponProperties()
    {
        // Access the weapon's properties directly from the script
        Debug.Log($"Weapon: {weaponName}, Damage: {damage}");
    }

    public void SwitchWeapon()
    {
        UpdateWeaponProperties();
    }
}
