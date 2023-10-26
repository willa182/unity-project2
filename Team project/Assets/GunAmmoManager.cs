using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public int pistolAmmoLimit = 15;
    public int shotgunAmmoLimit = 8;
    public int rifleAmmoLimit = 30;
    public int currentAmmo;
    public Animator reloadAnimator;

    private bool isReloading = false;
    public Transform playerRightHand;

    void Start()
    {
        // Find the weapon in the player's right hand during the start
        FindCurrentWeapon();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            Reload();
        }

        // Update the current weapon and ammo during gameplay
        FindCurrentWeapon();
    }

    public void Reload()
    {
        if (isReloading || currentAmmo == GetAmmoLimit())
        {
            return;
        }

        isReloading = true;
        reloadAnimator.SetTrigger("IsReloading");

        StartCoroutine(CompleteReload());
    }

    IEnumerator CompleteReload()
    {
        yield return new WaitForSeconds(2.0f);
        currentAmmo = GetAmmoLimit();
        isReloading = false;
    }

    int GetAmmoLimit()
    {
        if (IsPistol())
        {
            return pistolAmmoLimit;
        }
        else if (IsRifle())
        {
            return rifleAmmoLimit;
        }
        else if (IsShotgun())
        {
            return shotgunAmmoLimit;
        }
        return 0;
    }

    bool IsPistol()
    {
        return IsWeaponWithTag("Pistol");
    }

    bool IsRifle()
    {
        return IsWeaponWithTag("Rifle");
    }

    bool IsShotgun()
    {
        return IsWeaponWithTag("Shotgun");
    }

    bool IsWeaponWithTag(string tag)
    {
        return playerRightHand != null && playerRightHand.childCount > 0 &&
            playerRightHand.GetChild(0).CompareTag(tag);
    }

    void FindCurrentWeapon()
    {
        // Clear the current ammo by default
        currentAmmo = 0;

        // Check if there's a weapon in the player's right hand
        if (playerRightHand != null)
        {
            // Check each child of the right hand
            foreach (Transform child in playerRightHand)
            {
                string weaponTag = child.tag;

                // If the child's tag corresponds to a valid weapon, update currentAmmo
                if (weaponTag == "Pistol")
                {
                    currentAmmo = pistolAmmoLimit;
                    break; // No need to check further
                }
                else if (weaponTag == "Shotgun")
                {
                    currentAmmo = shotgunAmmoLimit;
                    break;
                }
                else if (weaponTag == "Rifle")
                {
                    currentAmmo = rifleAmmoLimit;
                    break;
                }
            }
        }
    }

    // Update currentAmmo when the player fires the weapon
   public void UpdateAmmoOnFire()
    {
        Debug.Log("Updating ammo on fire.");
        if (IsPistol())
        {
            currentAmmo--;
        }
        else if (IsShotgun())
        {
            currentAmmo--;
        }
        else if (IsRifle())
        {
            currentAmmo--;
        }
        Debug.Log("Current Ammo: " + currentAmmo);
    }
}
