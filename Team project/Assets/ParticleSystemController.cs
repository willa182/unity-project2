using System.Collections;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    private ParticleSystem particleWater;
    private bool isTriggered = false;

    private void Start()
    {
        // Get the Particle System component from the child GameObject
        particleWater = GetComponentInChildren<ParticleSystem>();

        // Ensure the Particle System is initially disabled
        particleWater.Stop();
        particleWater.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            // Enable the Particle System when the player enters the trigger
            particleWater.Play();

            // Set the flag to prevent further triggering
            isTriggered = true;

            // Start a coroutine to disable the Particle System after 10 seconds
            StartCoroutine(DisableParticleSystemAfterDelay(10f));
        }
    }

    private IEnumerator DisableParticleSystemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Disable the Particle System after the specified delay
        particleWater.Stop();
        particleWater.Clear();
    }
}
