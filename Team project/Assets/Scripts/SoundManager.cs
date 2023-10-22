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
    public AudioSource heartBeatAudioSource;
    public AudioSource exploAudioSource;
    public AudioSource flashLightAudioSource;
    public AudioSource outOfBreath;

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
    // Stop rifle fire sound
    public void StopRifleFireSound()
    {
        rifleAudioSource.Stop();
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

    // Play heartbeat sound
    public void PlayHeartBeatSound()
    {
        heartBeatAudioSource.Play();
    }
    // Stop heartbeat sound
    public void StopHeartBeatSound()
    {
        heartBeatAudioSource.Stop();
    }
    // Play Explo sound
    public void PlayExploSound()
    {
        exploAudioSource.Play();
    }
    public void PlayOnandOffSound()
    {
        flashLightAudioSource.Play();
    }
    public void PlayOutOfBreathSound()
    {
        outOfBreath.Play();
    }
    public void StopOutOfBreathSound()
    {
        outOfBreath.Stop();
    }
}
