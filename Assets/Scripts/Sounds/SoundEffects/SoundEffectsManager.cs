using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AreaSoundEffectsSO[] areaSoundEffectsSOs;

    [SerializeField]
    private MapCheckpoint[] mapCheckpoints;

    private int currentAreaID;
    private int lastMapID;
    private int lastRandom;

    private PlayerHealth player;

    // Start is called before the first frame update
    void Start()
    {
        currentAreaID = 0;
        lastMapID = 0;
        lastRandom = -1;
        audioSource = GetComponent<AudioSource>();
        for(int i  = 0; i < mapCheckpoints.Length; i++)
        {
            mapCheckpoints[i].OnSpawnNextMapPart += SoundEffectsManager_OnSpawnNextMapPart;
        }
        StartCoroutine(PlaySoundEffectWithRandomDelay(10));
        player = GameManager.Instance.GetPlayerReference().GetComponent<PlayerHealth>();
        player.OnDeath += Player_OnDeath;
    }

    private void Player_OnDeath()
    {
        StartCoroutine(GraduallyDecreaseVolume(audioSource,3));
    }

    private void OnDestroy()
    {
        for (int i = 0; i < mapCheckpoints.Length; i++)
        {
            mapCheckpoints[i].OnSpawnNextMapPart -= SoundEffectsManager_OnSpawnNextMapPart;
        }
        player.OnDeath -= Player_OnDeath;

    }

    private void SoundEffectsManager_OnSpawnNextMapPart(int nextAreaID)
    {
        if (nextAreaID == mapCheckpoints.Length)
        {
            StopAllCoroutines();
        }
        lastMapID = currentAreaID;
        currentAreaID = nextAreaID;
    }

    private IEnumerator PlaySoundEffectWithRandomDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomSound(currentAreaID);
        StartCoroutine(PlaySoundEffectWithRandomDelay(Random.Range(20,35)));
    }

    private IEnumerator GraduallyDecreaseVolume(AudioSource audioSource, float timeInSeconds)
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / timeInSeconds;
            yield return null;
        }
    }

    private void PlayRandomSound(int currentAreaID)
    {
        int rand = -1;
        if (lastMapID == currentAreaID)
        {
            do
            {
                rand = Random.Range(0, areaSoundEffectsSOs[currentAreaID].audioClips.Length);
            } while (rand == lastRandom);
        }
        else
        {
            rand = Random.Range(0, areaSoundEffectsSOs[currentAreaID].audioClips.Length);
        }
        
        lastRandom = rand;
        audioSource.PlayOneShot(areaSoundEffectsSOs[currentAreaID].audioClips[rand]);
    }
}
