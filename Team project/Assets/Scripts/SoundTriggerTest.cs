using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTriggerTest : MonoBehaviour
{
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
                soundManager.PlaySoundTriggerTest();
                hasBeenTriggered = true;
            }
        }
    }
}
