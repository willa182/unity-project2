using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCount : MonoBehaviour

{
    public AmmoManager ammoManager;



    void Update()
    {
        if (ammoManager != null)
        {
            // Assuming you have a Text component attached to this GameObject.
            GetComponent<Text>().text = "Ammo: " + ammoManager.currentAmmo.ToString();
        }
    }

}
