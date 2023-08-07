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
        audioSource.PlayOneShot(hellHoundAudioClipsSO.AttackAudioClipArray[Random.Range(0, hellHoundAudioClipsSO.AttackAudioClipArray.Length)], audioSource.volume);
    }

    private void HellHound_OnHellHoundAttack()
    {
        audioSource.PlayOneShot(hellHoundAudioClipsSO.BarkAudioClipArray[Random.Range(0, hellHoundAudioClipsSO.BarkAudioClipArray.Length)], audioSource.volume);
    }

    private void HellHound_OnAggressiveStateChange()
    {
        //audioSource.Stop();
    }

    private void PlayerDetection_OnPlayerInRange(PlayerMovement player)
    {
        StartCoroutine(DelayAfterPlayerDetected());
    }

    private IEnumerator DelayAfterPlayerDetected()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, 0.25f));

        audioSource.clip = hellHoundAudioClipsSO.GrowlThenBarkAudioClip;
        audioSource.Play();
    }
}
