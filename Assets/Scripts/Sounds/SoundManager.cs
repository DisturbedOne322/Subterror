using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource soundEffectsAudioSource;

    [SerializeField] 
    private AudioSource focusedLightAudioSource;

    [SerializeField] 
    private AudioSource focusedLightCoolingAudioSource;

    [SerializeField]
    private AudioSource airTimeAudioSource;


    [SerializeField]
    private AudioClipsSO audioClipsSO;

    [SerializeField]
    private AudioClip qteAudioClip;

    [SerializeField]
    private AudioClip saveGameAudio;

    [SerializeField]
    private AudioClip doorClose;
    [SerializeField] 
    private AudioClip doorOpen;
    [SerializeField]
    private AudioClip floorCracked;

    [SerializeField]
    private AudioClip[] headlightBrokenAudioClipArray;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        soundEffectsAudioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        QTE.instance.OnQTEStart += QTE_OnQTEStart;
    }

    private void OnDestroy()
    {
        QTE.instance.OnQTEStart -= QTE_OnQTEStart;
    }

    public void PlaySaveSound()
    {
        soundEffectsAudioSource.PlayOneShot(saveGameAudio);
    }

    public void PlayBossFightCloseDoorSound()
    {
        soundEffectsAudioSource.PlayOneShot(doorClose);
    }

    public void PlayDoorOpenSound()
    {
        soundEffectsAudioSource.PlayOneShot(doorOpen);
    }

    public void PlayFloorCracked()
    {
        soundEffectsAudioSource.PlayOneShot(floorCracked);
    }

    private void QTE_OnQTEStart()
    {
        soundEffectsAudioSource.PlayOneShot(qteAudioClip);
    }

    public void PlayShootSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.shootSound);
    }
    public void PlayMagInSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.magInSound);
    }
    public void PlayMagOutSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.magOutSound);
    }
    public void PlayReloadSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.reloadSound);
    }


    int lastGetHurtSound = -1;
    public void PlayGetHurtSound()
    {
        int rand;
        do
        {
            rand = Random.Range(0, audioClipsSO.getHurtSound.Length);
        } while (rand == lastGetHurtSound);

        lastGetHurtSound = rand;
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.getHurtSound[rand]);
    }

    public void PlayBoneCrackSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.boneCrackSound);
    }

    public void PlayFocusedLightSound(bool isFocusedLightEnabled)
    {
        if (isFocusedLightEnabled)
        {
            focusedLightAudioSource.PlayOneShot(audioClipsSO.focusedLightStartup);
            focusedLightCoolingAudioSource.Stop();
            focusedLightAudioSource.Play();
        }
        else
        {
            focusedLightAudioSource.Stop();
            focusedLightCoolingAudioSource.Play();
        }
    }

    public void PlayLandingSound(float volume)
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.landingSound, volume);
    }

    public void PlayNoAmmoSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.noAmmoSound);
    }

    public void PlayStepsSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.stepSound[Random.Range(0,audioClipsSO.stepSound.Length)]);
    }
    public void PlayMudStepsSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.stepMudSound[Random.Range(0, audioClipsSO.stepMudSound.Length)]);
    }

    public void PlayBrokenHeadlightSound()
    {
        soundEffectsAudioSource.PlayOneShot(headlightBrokenAudioClipArray[Random.Range(0,headlightBrokenAudioClipArray.Length)]);
    }

    public void PlayExhaleSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.exhaleSound);
    }
    public void PlayInhaleSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.inhaleSound);
    }
    public void PlayDeathSound()
    {
        soundEffectsAudioSource.PlayOneShot(audioClipsSO.deathSound);
    }

    private bool inAir = false;

    public void PlayAirTimeSound()
    {
        StartCoroutine(IncreaseVolumeAfterDelay());
    }
    
    private IEnumerator IncreaseVolumeAfterDelay()
    {
        if (!inAir)
        {
            inAir = true;   
            yield return new WaitForSeconds(1.5f);
            float volume = 0;
            airTimeAudioSource.Play();
            while (volume < 1)
            {
                volume += Time.deltaTime / 2;
                airTimeAudioSource.volume = volume;
                yield return null;
            }
        }
    }

    public void StopPlayingAirTime()
    {
        StopAllCoroutines();
        airTimeAudioSource.Stop();
        airTimeAudioSource.volume = 0;
        inAir = false;
    }
}
