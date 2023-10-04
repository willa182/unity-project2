using System.Collections;
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
    public float timeMultiplier = 60f; // Adjust this to control the speed of time

    private float currentTime = 8 * 60 * 60; // Start at 08:00 in seconds (24-hour format)
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
            // Start transitioning to night at 19:00
            isDay = false;
            StartCoroutine(TransitionDayNight());
            RenderSettings.skybox = nightSkyboxMaterial;
            directionalLight.enabled = false;
        }

        if (currentTime >= (24 * 60 * 60))
        {
            // Reset the timer at 24:00
            currentTime -= 24 * 60 * 60;
        }

        // Set day skybox at 07:59
        if (currentTime >= (7 * 60 * 60) && currentTime < (8 * 60 * 60) && !isDay)
        {
            isDay = true;
            RenderSettings.skybox = daySkyboxMaterial;
            directionalLight.enabled = true;
        }

        UpdateTimerText();

        // Rotate the directional light on the X-axis to simulate day/night cycle
        float angle = Mathf.Lerp(0, 180, Mathf.Clamp01(currentTime / (24 * 60 * 60)));
        directionalLight.transform.rotation = Quaternion.Euler(angle, 0, 0);
    }

    private IEnumerator TransitionDayNight()
    {
        float t = 0f;
        Color startColor = Color.white; // Day color
        Color endColor = Color.black; // Night color

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
}
