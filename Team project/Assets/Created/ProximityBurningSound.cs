using UnityEngine;

public class ProximityBurningSound : MonoBehaviour
{
    public AudioClip burningSoundClip;
    public AudioSource burningAudioSource;
    public Transform playerTransform;
    public float maxVolume = 1.0f;
    public float proximityDistance = 10f;

    private bool isPlayerNearby = false;

    private void Start()
    {
        // Make sure you have an AudioSource component attached to the object
        if (burningAudioSource == null)
        {
            burningAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // Assign the burning sound clip to the AudioSource
        if (burningSoundClip != null)
        {
            burningAudioSource.clip = burningSoundClip;
        }

        // Play the burning sound on loop
        if (burningAudioSource.clip != null)
        {
            burningAudioSource.loop = true;
        }
    }

    private void Update()
    {
        // Check the distance between the player and the object
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            float volume = Mathf.Lerp(0f, maxVolume, 1f - (distance / proximityDistance));
            burningAudioSource.volume = volume;

            if (distance <= proximityDistance && !isPlayerNearby)
            {
                // Player is close enough, start playing the burning sound
                burningAudioSource.Play();
                isPlayerNearby = true;
            }
            else if (distance > proximityDistance && isPlayerNearby)
            {
                // Player moved away, stop playing the burning sound
                burningAudioSource.Stop();
                isPlayerNearby = false;
            }
        }
    }
}
