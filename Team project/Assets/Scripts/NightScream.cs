using System.Collections;
using UnityEngine;

public class NightScream : MonoBehaviour
{
    public SoundManager soundManager;
    public float initialDelay = 10f; // Delay before the first sound
    public float timeBetweenSounds = 30f; // Adjust the time between sounds in seconds

    private void Start()
    {
        StartCoroutine(PlaySoundsWithDelay());
    }

    private IEnumerator PlaySoundsWithDelay()
    {
        yield return new WaitForSeconds(initialDelay);

        while (true)
        {
            // Play the desired sound from the Sound Manager
            soundManager.PlayNightscreamSound(); // Replace with the actual method to play your sound

            // Wait for the specified time before playing the sound again
            yield return new WaitForSeconds(timeBetweenSounds);
        }
    }
}
