using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    public static MainMenuSoundManager Instance;

    [SerializeField]
    private AudioClip woodenDoorOpenSound;
    [SerializeField]
    private AudioClip metalDoorOpenSound;

    [SerializeField]
    private AudioClip helpSignSound;
    [SerializeField]
    private AudioClip settingsSignSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public enum SoundType
    {
        WoodenDoor,
        MetalDoor,
        HelpSign,
        SettingsSign,
        NoSound
    }

    public void PlayAnimSound(SoundType type)
    {
        switch (type)
        {
            case SoundType.WoodenDoor:
                audioSource.PlayOneShot(woodenDoorOpenSound);
                break;
            case SoundType.MetalDoor:
                audioSource.PlayOneShot(metalDoorOpenSound);
                break;
            case SoundType.SettingsSign:
                audioSource.PlayOneShot(settingsSignSound);
                break;
            case SoundType.HelpSign:
                audioSource.PlayOneShot(helpSignSound);
                break;
        }
    }
}
