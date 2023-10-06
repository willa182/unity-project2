using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Custom/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Default Weapon";
    public float damage = 5f;
}

