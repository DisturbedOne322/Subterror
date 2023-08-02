using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhoshSoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip SpottedAudioClip;
    [SerializeField]
    private AudioClip BurstAttackAudioClip;

    private Ghost ghost;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ghost = GetComponent<Ghost>();
        ghost.OnBurstAttack += Ghost_OnBurstAttack;
        ghost.OnGhostSpotted += Ghost_OnGhostSpotted;
    }

    private void OnDestroy()
    {
        if(ghost!=null)
        {
            ghost.OnBurstAttack -= Ghost_OnBurstAttack;
            ghost.OnGhostSpotted -= Ghost_OnGhostSpotted;
        }
    }

    private void Ghost_OnGhostSpotted()
    {
        audioSource.PlayOneShot(SpottedAudioClip);
    }

    private void Ghost_OnBurstAttack()
    {
        audioSource.PlayOneShot(BurstAttackAudioClip);
    }
}
