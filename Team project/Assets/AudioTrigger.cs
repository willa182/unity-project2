using System.Collections;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip audioClip;
    private AudioSource audioSource;
    private bool isPlaying;

    public float minDistance = 5.0f; // Minimum distance for maximum volume.
    public float maxDistance = 10.0f; // Maximum distance at which sound is audible.
    public float minVolume = 0.1f; // Minimum volume when player is at maxDistance.

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = audioClip;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 1.0f; // Start with volume at maximum (1.0).
    }

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if (distance <= maxDistance)
            {
                float targetVolume = Mathf.Lerp(minVolume, 1.0f, Mathf.InverseLerp(maxDistance, minDistance, distance));
                audioSource.volume = targetVolume;

                if (!isPlaying)
                {
                    StartCoroutine(PlayAudioOnLoop());
                }
            }
            else
            {
                audioSource.volume = 0.0f;
                StopCoroutine(PlayAudioOnLoop());
            }
        }
    }

    IEnumerator PlayAudioOnLoop()
    {
        isPlaying = true;
        while (isPlaying)
        {
            audioSource.Play();
            yield return new WaitForSeconds(audioClip.length);
        }
    }
}
