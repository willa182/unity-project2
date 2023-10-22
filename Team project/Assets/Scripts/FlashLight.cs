using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    private Light flashlight;
    private SoundManager soundManager;
    private bool isToggling = false;

    void Start()
    {
        flashlight = GetComponent<Light>();
        soundManager = SoundManager.instance;
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
        if (Input.GetKeyDown(KeyCode.T) && !isToggling)
        {
            StartCoroutine(ToggleFlashlightWithDelay());
        }
    }

    IEnumerator ToggleFlashlightWithDelay()
    {
        isToggling = true;
        soundManager.PlayOnandOffSound();
        yield return new WaitForSeconds(0.5f); // Change the delay duration as needed
        ToggleFlashlight();
        isToggling = false;
    }

    void ToggleFlashlight()
    {
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
