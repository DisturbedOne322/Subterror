using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellHoundSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private HellHoundAudioClipsSO hellHoundAudioClipsSO;

    private HellHound hellHound;

    [SerializeField]
    private PlayerInRange playerDetection;

    private PlayerMovement player;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = hellHoundAudioClipsSO.EatingAudioClips[UnityEngine.Random.Range(0, hellHoundAudioClipsSO.EatingAudioClips.Length)];
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();

        hellHound = GetComponent<HellHound>();
        playerDetection.OnPlayerInRange += PlayerDetection_OnPlayerInRange;
        hellHound.OnAggressiveStateChange += HellHound_OnAggressiveStateChange;
        hellHound.OnHellHoundAttack += HellHound_OnHellHoundAttack;
        hellHound.OnSuccessfulHit += HellHound_OnSuccessfulHit;
    }
    private void OnDestroy()
    {
        playerDetection.OnPlayerInRange -= PlayerDetection_OnPlayerInRange;
        hellHound.OnAggressiveStateChange -= HellHound_OnAggressiveStateChange;
        hellHound.OnHellHoundAttack -= HellHound_OnHellHoundAttack;
        hellHound.OnSuccessfulHit -= HellHound_OnSuccessfulHit;
    }

    private void HellHound_OnSuccessfulHit()
    {
        audioSource.PlayOneShot(hellHoundAudioClipsSO.AttackAudioClipArray[UnityEngine.Random.Range(0, hellHoundAudioClipsSO.AttackAudioClipArray.Length)], audioSource.volume);
    }

    private void HellHound_OnHellHoundAttack()
    {
        audioSource.PlayOneShot(hellHoundAudioClipsSO.BarkAudioClipArray[UnityEngine.Random.Range(0, hellHoundAudioClipsSO.BarkAudioClipArray.Length)], audioSource.volume);
    }

    private void HellHound_OnAggressiveStateChange()
    {
        //audioSource.Stop();
    }

    private void PlayerDetection_OnPlayerInRange(PlayerMovement player)
    {
        StartCoroutine(PlayDelayedAfterPlayerDetected());
    }

    private IEnumerator PlayDelayedAfterPlayerDetected()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, 0.5f));

        audioSource.clip = hellHoundAudioClipsSO.GrowlThenBarkAudioClip[UnityEngine.Random.Range(0, hellHoundAudioClipsSO.GrowlThenBarkAudioClip.Length)];
        audioSource.Play();
    }
}
