using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButtonClickSound:MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip audioClip;
    public void PlayButtonSoundOnClick()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
