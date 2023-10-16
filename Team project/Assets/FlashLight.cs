using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    private Light flashlight;

    void Start()
    {
        // Get the Light component attached to this GameObject
        flashlight = GetComponent<Light>();

        // Ensure the light starts off
        if (flashlight != null)
        {
            flashlight.enabled = false;
        }
        else
        {
            Debug.LogError("Light component not found!");
        }
    }

    void Update()
    {
        // Check for player input (G key)
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        // Toggle the enabled state of the flashlight
        if (flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }
        else
        {
            Debug.LogError("Light component not found!");
        }
    }
}
