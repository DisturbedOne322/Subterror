using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundManager : MonoBehaviour
{
    [SerializeField]
    private MainMenuManager mainMenuManager;
    [SerializeField]
    private AudioSource effectsAudioSource;
    [SerializeField]
    private AudioSource musicAudioSource;

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
    private float soundCD = 0.5f;

    private void Awake()
    {
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

    private void Start()
    {
        mainMenuManager.OnExitScene += MainMenuManager_OnExitScene;
    }

    private void MainMenuManager_OnExitScene()
    {
        StartCoroutine(DecreaseVolumeOnExitScene());   
    }

    private IEnumerator DecreaseVolumeOnExitScene()
    {
        float duration = 1;
        while(duration > 0)
        {
            musicAudioSource.volume = duration;
            duration -= Time.deltaTime / 4;
            yield return null;
        }
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
                effectsAudioSource.PlayOneShot(woodenDoorOpenSound);
                break;
            case SoundType.MetalDoor:
                effectsAudioSource.PlayOneShot(metalDoorOpenSound);
                break;
            case SoundType.SettingsSign:
                effectsAudioSource.PlayOneShot(settingsSignSound);
                break;
            case SoundType.HelpSign:
                effectsAudioSource.PlayOneShot(helpSignSound);
                break;
            case SoundType.CreditsSign:
                effectsAudioSource.PlayOneShot(creditsSignSound);
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
