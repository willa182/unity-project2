using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public int pistolAmmo = 15;
    public int shotgunAmmo = 8;
    public int rifleAmmo = 30;

    public int pistolAmmoReserve = 180;
    public int shotgunAmmoReserve = 80;
    public int rifleAmmoReserve = 150;

    public static AmmoManager instance;

    private void Awake()
    {
        instance = this;
    }


    public int GetAmmoCount(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                return pistolAmmo;
            case "Shotgun":
                return shotgunAmmo;
            case "Rifle":
                return rifleAmmo;
            default:
                return 0;
        }
    }

    public int GetAmmoReserve(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                return pistolAmmoReserve;
            case "Shotgun":
                return shotgunAmmoReserve;
            case "Rifle":
                return rifleAmmoReserve;
            default:
                return 0;
        }
    }

    public bool CanShoot(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                return pistolAmmo > 0;
            case "Shotgun":
                return shotgunAmmo > 0;
            case "Rifle":
                return rifleAmmo > 0;
            default:
                return false;
        }
    }

    public void Shoot(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                if (pistolAmmo > 0)
                {
                    pistolAmmo--;
                    Debug.Log("Pistol ammo used. Main Ammo: " + pistolAmmo + ", Reserve Ammo: " + pistolAmmoReserve);
                }
                break;
            case "Shotgun":
                shotgunAmmo--;
                break;
            case "Rifle":
                rifleAmmo--;
                break;
        }
    }

    public bool NeedsReloading(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                return pistolAmmo == 0;
            case "Shotgun":
                return shotgunAmmo == 0;
            case "Rifle":
                return rifleAmmo == 0;
            default:
                return false;
        }
    }

    public void Reload(string weaponType, int ammoCount)
    {
        switch (weaponType)
        {
            case "Pistol":
                int pistolRoundsToReload = Mathf.Min(pistolAmmoReserve, ammoCount);
                pistolAmmo += pistolRoundsToReload;
                pistolAmmoReserve -= pistolRoundsToReload;
                Debug.Log("Pistol reloaded. Main Ammo: " + pistolAmmo + ", Reserve Ammo: " + pistolAmmoReserve);
                break;
            case "Shotgun":
                int shotgunRoundsToReload = Mathf.Min(shotgunAmmoReserve, 8 - shotgunAmmo); // Calculate how many rounds can be reloaded
                shotgunAmmo += shotgunRoundsToReload; // Add the rounds to the shotgun's ammo
                shotgunAmmoReserve -= shotgunRoundsToReload; // Deduct the rounds from the reserve
                break;
            case "Rifle":
                int rifleRoundsToReload = Mathf.Min(rifleAmmoReserve, 30 - rifleAmmo); // Calculate how many rounds can be reloaded
                rifleAmmo += rifleRoundsToReload; // Add the rounds to the rifle's ammo
                rifleAmmoReserve -= rifleRoundsToReload; // Deduct the rounds from the reserve
                break;
        }
    }

    public bool CanReload(string weaponType)
    {
        switch (weaponType)
        {
            case "Pistol":
                return pistolAmmo < 15 && pistolAmmoReserve > 0;
            case "Shotgun":
                return shotgunAmmo < 8 && shotgunAmmoReserve > 0;
            case "Rifle":
                return rifleAmmo < 30 && rifleAmmoReserve > 0;
            default:
                return false;
        }
    }

    public void PickUpAmmo(Collider ammoCollider)
    {
        string ammoTag = ammoCollider.tag;

        // Check the ammo tag and add the appropriate amount of ammo to the reserve
        switch (ammoTag)
        {
            case "PistolAmmo":
                pistolAmmoReserve += Random.Range(10, 26); // Add random ammo between 10 and 25
                break;
            case "RifleAmmo":
                rifleAmmoReserve += Random.Range(10, 26);
                break;
            case "ShotgunAmmo":
                shotgunAmmoReserve += Random.Range(10, 26);
                break;
            default:
                break;
        }

        // Destroy the picked-up ammo
        Destroy(ammoCollider.gameObject);
    }
}
