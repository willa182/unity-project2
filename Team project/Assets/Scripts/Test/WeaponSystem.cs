using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public GameObject[] weapons; // Reference to the individual weapon objects.
    private Weapons currentWeapon; // Reference to the currently equipped weapon.
    private GunFires shootingScript; //

    private int currentWeaponIndex = 0;

    private void Start()
    {
        shootingScript = GetComponent<GunFires>(); //
        EquipWeapon(currentWeaponIndex); // Equip the initial weapon.
    }

    // Function to switch to the next weapon
    public void SwitchToNextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        EquipWeapon(currentWeaponIndex);
    }

    // Function to switch to a specific weapon by index
    public void SwitchToWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            currentWeaponIndex = weaponIndex;
            EquipWeapon(currentWeaponIndex);
        }
    }

    // Function to get the properties of the currently equipped weapon
    public void GetCurrentWeaponProperties(out string weaponName, out float damage)
    {
        if (currentWeapon != null)
        {
            // Access the weapon's properties through the Weapons script
            weaponName = currentWeapon.weaponName;
            damage = currentWeapon.damage;
        }
        else
        {
            weaponName = "Default Weapon";
            damage = 0f;
        }
    }

    // Equip a weapon by index and update its properties
    private void EquipWeapon(int weaponIndex)
    {
        // Deactivate the currently equipped weapon, if any.
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        // Activate the new weapon.
        currentWeapon = weapons[weaponIndex].GetComponent<Weapons>();
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Current weapon instance not found.");
        }
    }
}
