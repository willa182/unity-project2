using UnityEngine;

public class FlashingStreetLight : MonoBehaviour
{
    public Light streetLight;
    public float flashingSpeed = 0.5f;

    private float timer;
    private bool isLightOn;

    private void Start()
    {
        timer = 0f;
        isLightOn = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Change the light based on the flashing speed
        if (timer >= flashingSpeed)
        {
            streetLight.enabled = !isLightOn;
            isLightOn = !isLightOn;

            timer = 0f;
        }
    }
}
