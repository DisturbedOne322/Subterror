using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    private IsPlayerInRange isPlayerInRange;

    [SerializeField]
    private AudioClip tryOpenDoorSound;

    [SerializeField]
    private AudioClip openDoorSound;

    private AudioSource audioSource;

    private bool inRange = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        isPlayerInRange = GetComponent<IsPlayerInRange>();
    }

    private void Start()
    {
        InputManager.Instance.OnInteract += Instance_OnInteract;
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;

        MageBoss.OnFightFinished += MageBoss_OnFightFinished;
    }

    private bool finishGame = false;

    private void MageBoss_OnFightFinished()
    {
        finishGame = true;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        inRange = obj;
    }

    private void Instance_OnInteract()
    {
        if(inRange && !inCD)
        {
            if(!finishGame)
            {
                audioSource.PlayOneShot(tryOpenDoorSound);
                StartCoroutine(Cooldown());
            }
            else
            {
                //load game scene with credits
                audioSource.PlayOneShot(openDoorSound);
                StartCoroutine(Cooldown());
            }
        }
    }

    private bool inCD = false;

    private IEnumerator Cooldown()
    {
        inCD = true;
        yield return new WaitForSeconds(2f);
        inCD = false;
    }
}
