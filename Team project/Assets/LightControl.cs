using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    public GameObject lightsParent;
    private List<Light> lights = new List<Light>();
    private bool isFlashing = false;
    public float flashInterval = 0.3f;
    public float lightsOnDuration = 5f;
    public float lightDisabledDuration = 7f;

    void Start()
    {
        // Find all the lights in the children of the lightsParent object
        foreach (Transform child in lightsParent.transform)
        {
            Light childLight = child.GetComponent<Light>();
            if (childLight != null)
            {
                lights.Add(childLight);
                childLight.enabled = true; // Enable all lights initially
            }
        }

        StartCoroutine(LightControlLoop());
    }

    IEnumerator LightControlLoop()
    {
        while (true)
        {
            // Enable all lights for 5 seconds
            foreach (Light light in lights)
            {
                light.enabled = true;
            }

            yield return new WaitForSeconds(lightsOnDuration);

            // Disable a random light
            int randomIndex = Random.Range(0, lights.Count);
            lights[randomIndex].enabled = false;

            yield return new WaitForSeconds(lightDisabledDuration);

            // Flash all lights for 10 seconds
            StartCoroutine(FlashLights(10f));

            yield return new WaitForSeconds(10f);

            // Enable all lights again
            foreach (Light light in lights)
            {
                light.enabled = true;
            }

            yield return new WaitForSeconds(1f); // Adjust this delay if needed
        }
    }

    IEnumerator FlashLights(float duration)
    {
        isFlashing = true;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foreach (Light light in lights)
            {
                light.enabled = !light.enabled; // Toggle the state of each light
            }

            elapsedTime += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        // Ensure all lights are enabled at the end of the flash
        foreach (Light light in lights)
        {
            light.enabled = true;
        }

        isFlashing = false;
    }
}
