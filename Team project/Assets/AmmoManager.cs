using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public int pistolAmmo = 15;
    public int shotgunAmmo = 8;
    public int rifleAmmo = 30;

    private void Start()
    {
        // You can set initial ammo counts here or load them from a save file.
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
                pistolAmmo--;
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
                pistolAmmo += ammoCount;
                break;
            case "Shotgun":
                shotgunAmmo += ammoCount;
                break;
            case "Rifle":
                rifleAmmo += ammoCount;
                break;
        }
    }
}
