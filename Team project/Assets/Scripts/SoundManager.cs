using System.Collections.Generic;
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
    public AudioSource grenadeExplo;
    public AudioSource pistolReload;
    public AudioSource rifleReload;
    public AudioSource shotgunReload;
    public AudioSource zombieMoan;
    public AudioSource zombieGrunt;
    public AudioSource zombieHiss;
    public AudioSource zombieGrowl;
    public AudioSource zombieDeath;
    public AudioSource zombieChase;
    public AudioSource zombieAggresive;
    public AudioSource jetTriggerTest;//
    public AudioSource distanceTriggerTest;//

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
    public void PlayGrenadeExplo()
    {
        grenadeExplo.Play();
    }

    public void PlayPistolReload()
    {
        pistolReload.Play();
    }

    public void PlayRifleReload()
    {
        rifleReload.Play();
    }

    public void PlayShotgunReload()
    {
        shotgunReload.Play();
    }


    public void PlayZombieDeath()
    {
        zombieDeath.Play();
    }


    public void PlayRandomZombieSound()
    {
        AudioSource[] zombieSounds = { zombieMoan, zombieGrunt, zombieHiss, zombieGrowl };

        int randomIndex = Random.Range(0, zombieSounds.Length);
        zombieSounds[randomIndex].Play();
    }

    public void PlayRandomZombieChaseSound()
    {
        AudioSource[] zombieSounds = { zombieAggresive, zombieChase };

        int randomIndex = Random.Range(0, zombieSounds.Length);
        zombieSounds[randomIndex].Play();
    }


    public void PlaySoundTriggerTest()
    {
        jetTriggerTest.Play();
    }

    public void PlaySoundTriggerTest2()
    {
        distanceTriggerTest.Play();
    }
}
