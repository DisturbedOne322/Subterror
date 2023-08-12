using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource1;
    [SerializeField] 
    private AudioSource audioSource2;

    private AudioSource activeSource;

    [SerializeField]
    private Checkpoints[] checkpoints;

    private float changeBGMusicTime = 3f;

    [SerializeField]
    private AudioClip[] bgMusicArray;


    [SerializeField]
    private EnemySpawnManager enemySpawnManager;

    [SerializeField]
    private AudioClip executionerBossFightBGMusic;

    [SerializeField]
    private AudioClip bossFightBGMusic;

    [SerializeField]
    private AudioClip postBossFightBGMusic;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].OnChangeBGMusic += BGMusicManager_OnChangeBGMusic;
        }

        enemySpawnManager.OnMiniBossFightStarted += EnemySpawnManager_OnMiniBossFightStarted;
        enemySpawnManager.OnBossFightFinished += EnemySpawnManager_OnBossFightFinished;

        InitiateBossfight.OnBossFightInitiated += InitiateBossfight_OnBossFightInitiated;
        MageBoss.OnFightFinished += MageBoss_OnFightFinished;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].OnChangeBGMusic -= BGMusicManager_OnChangeBGMusic;
        }
        enemySpawnManager.OnMiniBossFightStarted -= EnemySpawnManager_OnMiniBossFightStarted;
        enemySpawnManager.OnBossFightFinished -= EnemySpawnManager_OnBossFightFinished;

        InitiateBossfight.OnBossFightInitiated -= InitiateBossfight_OnBossFightInitiated;
        MageBoss.OnFightFinished -= MageBoss_OnFightFinished;
    }

    private void MageBoss_OnFightFinished()
    {
        StartCoroutine(PlayWithDelay(10));
    }

    private void InitiateBossfight_OnBossFightInitiated()
    {
        activeSource.volume = 0;
        activeSource.clip = bossFightBGMusic;
        activeSource.Play();
        StartCoroutine(GraduallyIncreaseVolume(activeSource, changeBGMusicTime));
    }

    private IEnumerator PlayWithDelay(float delay)
    {
        StartCoroutine(GraduallyDecreaseVolume(activeSource, delay));
        activeSource = activeSource == audioSource1 ? audioSource2 : audioSource1;
        yield return new WaitForSeconds(delay);
        activeSource.clip = postBossFightBGMusic;
        activeSource.Play();
        StartCoroutine(GraduallyIncreaseVolume(activeSource, delay));
    }

    private void EnemySpawnManager_OnBossFightFinished()
    {
        activeSource.volume = 0;

        activeSource.clip = bgMusicArray[3];
        activeSource.Play();
        StartCoroutine(GraduallyIncreaseVolume(activeSource, changeBGMusicTime));
    }

    private void EnemySpawnManager_OnMiniBossFightStarted()
    {
        activeSource.clip = executionerBossFightBGMusic;
        activeSource.Play();
    }

    private void BGMusicManager_OnChangeBGMusic(int id)
    {
        InitiateFirstAudioSource(id);
        SwapActiveAudioSource(id);
    }

    private void InitiateFirstAudioSource(int id)
    {
        if (activeSource == null)
        {
            activeSource = audioSource1;
            activeSource.clip = bgMusicArray[id];
            activeSource.Play();
            return;
        }
    }
    private void SwapActiveAudioSource(int id)
    {
        StartCoroutine(GraduallyDecreaseVolume(activeSource, changeBGMusicTime));
        activeSource = activeSource == audioSource1 ? audioSource2: audioSource1;
        activeSource.clip = bgMusicArray[id];
        activeSource.Play();
        StartCoroutine(GraduallyIncreaseVolume(activeSource, changeBGMusicTime));
    }


    private IEnumerator GraduallyIncreaseVolume(AudioSource audioSource, float timeInSeconds)
    {
        while(audioSource.volume < 1) 
        {
            audioSource.volume += Time.deltaTime / timeInSeconds;
            yield return null;
        }
    }

    private IEnumerator GraduallyDecreaseVolume(AudioSource audioSource, float timeInSeconds)
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / timeInSeconds;
            yield return null;
        }
    }
}
