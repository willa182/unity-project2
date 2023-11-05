using UnityEngine;

public class FlashingLights : MonoBehaviour
{
    public Light redLight;
    public Light blueLight;
    public float flashingSpeed = 0.5f;

    private float timer;
    private bool isRedLightOn;

    private void Start()
    {
        timer = 0f;
        isRedLightOn = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Change the lights based on the flashing speed
        if (timer >= flashingSpeed)
        {
            if (isRedLightOn)
            {
                redLight.enabled = false;
                blueLight.enabled = true;
                isRedLightOn = false;
            }
            else
            {
                redLight.enabled = true;
                blueLight.enabled = false;
                isRedLightOn = true;
            }

            timer = 0f;
        }
    }
}
