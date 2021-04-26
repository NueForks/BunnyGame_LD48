using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicCutter : MonoBehaviour
{
    public AudioMixer mixer;
    public bool cut;
    float baseVolume = -6f;
    public GameObject cutFeedback;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (cut)
            {
                cutFeedback.SetActive(false);
                mixer.SetFloat("MasterVolume", baseVolume);
                cut = false;
            }
            else
            {
                cutFeedback.SetActive(true);
                mixer.SetFloat("MasterVolume", -80f);
                cut = true;
            }
        }
    }
}
