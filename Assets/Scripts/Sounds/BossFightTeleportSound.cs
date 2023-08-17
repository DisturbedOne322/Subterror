using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossFightTeleportSound : MonoBehaviour
{
    [SerializeField]
    private GameObject teleport;

    [SerializeField]
    private AudioClip[] audioClips;
    private string[] subs = new string[5];

    [SerializeField]
    private UIBossSubtitles[] subtitlePlaces;

    private AudioSource audioSource;

    private int indexToPlay;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        indexToPlay = 0;
        subs[0] = "You cannot run...";
        subs[1] = "I said you cannot escape...";
        subs[2] = "Turn back...";
        subs[3] = "Remember what you came for...";
        subs[4] = "Please, fight...";

        MageBoss.OnFightFinished += MageBoss_OnFightFinished;
    }

    private void MageBoss_OnFightFinished()
    {
        teleport.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (indexToPlay < 2)
        {
            audioSource.PlayOneShot(audioClips[indexToPlay]);
            DisplaySubtitles(indexToPlay);
            indexToPlay++;
        }
        else
        {
            int lastIndex = indexToPlay;
            do
            {
                indexToPlay = Random.Range(2, audioClips.Length);
            }while(indexToPlay == lastIndex);

            audioSource.PlayOneShot(audioClips[indexToPlay]);
            DisplaySubtitles(indexToPlay);
        }
    }

    private int randPlace = -1;

    private void DisplaySubtitles(int subTextIndex)
    {
        int lastPlace = randPlace;
        do
        {
            randPlace = Random.Range(0, subtitlePlaces.Length);
        } while (lastPlace == randPlace);

        subtitlePlaces[randPlace].SetText(subs[subTextIndex]);
    }
}
