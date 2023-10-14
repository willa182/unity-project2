using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Text timerText;
    public Light directionalLight;
    public Material daySkyboxMaterial;
    public Material nightSkyboxMaterial;
    public float dayDurationInSeconds = 60f;
    public float transitionDurationInSeconds = 5f;
    public float timeMultiplier = 60f;

    public List<Light> spotlights;

    private float currentTime = 8 * 60 * 60;
    private bool isDay = true;

    private void Start()
    {
        UpdateTimerText();
    }

    private void Update()
    {
        currentTime += Time.deltaTime * timeMultiplier;

        if (currentTime >= (19 * 60 * 60) && isDay)
        {
            isDay = false;
            StartCoroutine(TransitionDayNight());
            RenderSettings.skybox = nightSkyboxMaterial;
            directionalLight.enabled = false;
            SetSpotlightsActive(true);
        }

        if (currentTime >= (24 * 60 * 60))
        {
            currentTime -= 24 * 60 * 60;
        }

        if (currentTime >= (7 * 60 * 60) && currentTime < (8 * 60 * 60) && !isDay)
        {
            isDay = true;
            RenderSettings.skybox = daySkyboxMaterial;
            directionalLight.enabled = true;
            SetSpotlightsActive(false); 
        }

        UpdateTimerText();

        float angle = Mathf.Lerp(0, 180, Mathf.Clamp01(currentTime / (24 * 60 * 60)));
        directionalLight.transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    private IEnumerator TransitionDayNight()
    {
        float t = 0f;
        Color startColor = Color.white;
        Color endColor = Color.black;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionDurationInSeconds;
            RenderSettings.ambientLight = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
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
