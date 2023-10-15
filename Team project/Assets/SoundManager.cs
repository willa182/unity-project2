using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // Define background audio source
    public AudioSource backgroundAudioSource;

    // Define separate AudioSources for each weapon type
    public AudioSource pistolAudioSource;
    public AudioSource rifleAudioSource;
    public AudioSource shotgunAudioSource;
    public AudioSource nightscreamAudioSource;

    void Awake()
    {
        // Singleton pattern to ensure only one instance of SoundManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Set the background music
        // (Assuming background music is set directly in the Unity Editor)
    }

    // Play pistol fire sound
    public void PlayPistolFireSound()
    {
        pistolAudioSource.Play();
    }

    // Play rifle fire sound
    public void PlayRifleFireSound()
    {
        rifleAudioSource.Play();
    }

    // Play shotgun fire sound
    public void PlayShotgunFireSound()
    {
        shotgunAudioSource.Play();
    }

    public void PlayNightscreamSound()
    {
        nightscreamAudioSource.Play();
    }
}
