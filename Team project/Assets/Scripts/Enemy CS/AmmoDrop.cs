using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : MonoBehaviour
{
    public GameObject pistolAmmoPrefab;
    public GameObject rifleAmmoPrefab;
    public GameObject shotgunAmmoPrefab;

    public void EnemyKilled()
    {
        // Generate a random number between 0 and 1
        float randomValue = Random.value;

        // Check if the random value is less than or equal to 0.5 (50% chance)
        if (randomValue <= 1f)
        {
            DropAmmo();
            Debug.Log("Ammo Dropped");
        }
    }

    private void DropAmmo()
    {
        // Generate a random number between 0 and 3 to determine which ammo to drop
        int randomAmmoType = Random.Range(0, 3);

        GameObject ammoPrefab = null;
        string ammoTag = "";

        switch (randomAmmoType)
        {
            case 0:
                ammoPrefab = pistolAmmoPrefab;
                ammoTag = "PistolAmmo";
                break;
            case 1:
                ammoPrefab = rifleAmmoPrefab;
                ammoTag = "RifleAmmo";
                break;
            case 2:
                ammoPrefab = shotgunAmmoPrefab;
                ammoTag = "ShotgunAmmo";
                break;
            default:
                break;
        }

        if (ammoPrefab != null)
        {
            GameObject droppedAmmo = Instantiate(ammoPrefab, transform.position, Quaternion.identity);
            droppedAmmo.tag = ammoTag; // Set the tag of the instantiated ammo
        }
    }

}
