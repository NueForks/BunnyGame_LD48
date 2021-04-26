using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;

    public List<AudioClip> clips = new List<AudioClip>();


    public void PlaySound(int clipID)
    {
        audioSource.clip = clips[clipID];
        if(audioSource.isActiveAndEnabled)
        {
            audioSource.Play();
        }
    }
}
