using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDropSoundOnParticleCollision : MonoBehaviour
{
    private ParticleSystem particleSys;

    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;

    private void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    private bool canPlay = true;
    private IEnumerator Cooldown()
    {
        canPlay = false;
        yield return new WaitForSeconds(0.5f);
        canPlay = true;
    }

    private void OnParticleCollision(GameObject other)
    {
        if (audioSource.enabled && canPlay)
        {
            audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
            StartCoroutine(Cooldown());
        }
    }
}
