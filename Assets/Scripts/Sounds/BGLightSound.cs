using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGLightSound : MonoBehaviour
{
    private FlickeringLight flickeringLight;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip flickerFadeInSound;
    [SerializeField]
    private AudioClip flickerFadeOutSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (flickeringLight != null)
        {
            flickeringLight.OnFlicker += FlickeringLight_OnFlicker;
            flickeringLight.OnRestore += FlickeringLight_OnRestore;
        }
    }

    private void OnDestroy()
    {
        if (flickeringLight != null)
        {
            flickeringLight.OnFlicker -= FlickeringLight_OnFlicker;
            flickeringLight.OnRestore -= FlickeringLight_OnRestore;
        }
    }

    private void FlickeringLight_OnRestore()
    {
        if (!audioSource.enabled)
            return;
        audioSource.PlayOneShot(flickerFadeInSound);
        audioSource.PlayDelayed(1);
    }

    private void FlickeringLight_OnFlicker()
    {
        if (!audioSource.enabled)
            return;
        audioSource.Stop();
        audioSource.PlayOneShot(flickerFadeOutSound);
    }
}
