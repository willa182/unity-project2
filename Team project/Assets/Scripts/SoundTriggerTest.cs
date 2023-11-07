using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    public enum SoundType
    {
        JetSound,
        DistanceSound,
        // Add more sound types as needed
    }

    public SoundType soundToPlay;
    private SoundManager soundManager;
    private bool hasBeenTriggered = false;

    void Start()
    {
        soundManager = SoundManager.instance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasBeenTriggered && other.CompareTag("Player"))
        {
            if (soundManager != null)
            {
                PlaySelectedSound();
                hasBeenTriggered = true;
            }
        }
    }

    void PlaySelectedSound()
    {
        switch (soundToPlay)
        {
            case SoundType.JetSound:
                soundManager.PlaySoundTriggerTest();
                break;
            case SoundType.DistanceSound:
                soundManager.PlaySoundTriggerTest2();
                break;
                // Add more cases for additional sound types
        }
    }
}
