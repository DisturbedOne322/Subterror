using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicVolume : MonoBehaviour
{
    private PlayerMovement player;
    private AudioSource audioSource;
    private float maxDistance = 8f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.volume = DynamicSoundVolume.GetDynamicVolume(maxDistance, Vector2.Distance(transform.position, player.transform.position));
    }
}
