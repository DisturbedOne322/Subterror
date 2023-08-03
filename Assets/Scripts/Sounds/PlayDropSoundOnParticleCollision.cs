using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDropSoundOnParticleCollision : MonoBehaviour
{
    private ParticleSystem particleSys;

    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;

    private PlayerMovement player;

    private float maxDistanceSound = 20;

    private void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayerReference();
    }

    private void OnParticleCollision(GameObject other)
    {
        audioSource.volume = DynamicSoundVolume.GetDynamicVolume(maxDistanceSound, Vector2.Distance(other.transform.position, player.transform.position));
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}
