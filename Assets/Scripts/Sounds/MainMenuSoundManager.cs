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
    [SerializeField]
    private AudioClip creditsSignSound;

    private Dictionary<SoundType, float> soundTypeCooldownDict;
    private float soundCD = 0.33f;

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
        soundTypeCooldownDict = new Dictionary<SoundType, float>() {
            { SoundType.WoodenDoor, 0 },
            { SoundType.MetalDoor, 0 },
            { SoundType.HelpSign, 0 },
            { SoundType.SettingsSign, 0 },
            { SoundType.CreditsSign, 0 },
            { SoundType.NoSound, 0 },
        };
    }

    public enum SoundType
    {
        WoodenDoor,
        MetalDoor,
        HelpSign,
        SettingsSign,
        CreditsSign,
        NoSound
    }

    public void PlayAnimSound(SoundType type)
    {
        if (soundTypeCooldownDict[type] != 0)
            return;
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
            case SoundType.CreditsSign:
                audioSource.PlayOneShot(creditsSignSound);
                break;
        }
        StartCoroutine(SoundCooldown(type));
    }

    private IEnumerator SoundCooldown(SoundType soundType)
    {
        if (soundTypeCooldownDict[soundType] == 0)
        {
            soundTypeCooldownDict[soundType] = soundCD;
            yield return new WaitForSeconds(soundCD);
            soundTypeCooldownDict[soundType] = 0;
        }
    }
}
