using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudSlidingSound : MonoBehaviour
{
    private AudioSource audioSource;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            audioSource.Play();
            collision.gameObject.GetComponent<PlayerVisuals>().PlayMudSlidingParticles();
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.Stop();
            collision.gameObject.GetComponent<PlayerVisuals>().StopMudSlidingParticles();
        }
    }
}
