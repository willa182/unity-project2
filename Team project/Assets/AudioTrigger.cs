using System.Collections;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    public AudioClip audioClip;
    private AudioSource audioSource;
    private bool isPlaying;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float volume = 1 - (other.transform.position - transform.position).magnitude / 20;
            volume = Mathf.Clamp01(volume);
            audioSource.volume = volume;
            if (!isPlaying)
            {
                StartCoroutine(PlayAudioOnLoop());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCoroutine(PlayAudioOnLoop());
            audioSource.Stop();
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