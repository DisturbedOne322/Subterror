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
    }

    private void SoundEffectsManager_OnSpawnNextMapPart(int nextAreaID)
    {
        lastMapID = currentAreaID;
        currentAreaID = nextAreaID;
    }

    private IEnumerator PlaySoundEffectWithRandomDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayRandomSound(currentAreaID);
        StartCoroutine(PlaySoundEffectWithRandomDelay(Random.Range(20,35)));
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
