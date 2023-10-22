using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Text timerText;
    public Light directionalLight;
    public Material nightSkyboxMaterial; // Only the night skybox is needed

    public List<Light> spotlights;

    private float currentTime = 0; // Start at night

    private void Start()
    {
        RenderSettings.skybox = nightSkyboxMaterial; // Set the night skybox
        directionalLight.enabled = false; // Turn off directional light at night
        SetSpotlightsActive(true); // Enable spotlights
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        int hours = Mathf.FloorToInt(currentTime / 3600);
        int minutes = Mathf.FloorToInt((currentTime % 3600) / 60);
        string timeString = string.Format("{0:00}:{1:00}", hours, minutes);
        timerText.text = timeString;
    }

    private void SetSpotlightsActive(bool isActive)
    {
        foreach (Light spotlight in spotlights)
        {
            spotlight.enabled = isActive;
        }
    }
}
