using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    private IsPlayerInRange isPlayerInRange;

    [SerializeField]
    private AudioClip tryOpenDoorSound;
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
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnInteract -= Instance_OnInteract;
        isPlayerInRange.OnPlayerInRange -= IsPlayerInRange_OnPlayerInRange;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        inRange = obj;
    }

    private void Instance_OnInteract()
    {
        if(inRange && !inCD)
        {
            audioSource.PlayOneShot(tryOpenDoorSound);
            StartCoroutine(Cooldown());
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
