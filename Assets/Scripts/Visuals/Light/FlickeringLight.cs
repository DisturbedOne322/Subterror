using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;

public class FlickeringLight : MonoBehaviour
{
    private float flickerChance = 0.2f;
    private float flickerTryDelay = 10f;
    private float restoreLightIntensitySpeed = 0.05f;

    private Light2D light2D;

    private float delayBetweenRestoreLight = 5;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip flickerFadeInSound;
    [SerializeField]
    private AudioClip flickerFadeOutSound;


    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        light2D = GetComponent<Light2D>();
    }

    IEnumerator TryFlickerLight()
    {
        while (true)
        {
            if (Random.Range(0, 1f) < flickerChance)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(flickerFadeOutSound);
                light2D.intensity = 0;
                StartCoroutine(DelayBetweenLightRestore(delayBetweenRestoreLight));
            }
            yield return new WaitForSeconds(flickerTryDelay);
        }
    }

    IEnumerator DelayBetweenLightRestore(float delay)
    {
        yield return new WaitForSeconds(delay);

        StartCoroutine(RestoreLight());
    }

    IEnumerator RestoreLight()
    {
        audioSource.PlayOneShot(flickerFadeInSound);
        for (float i = 0; i <= 1; i += restoreLightIntensitySpeed)
        {
            light2D.intensity = i;
            yield return new WaitForSeconds(restoreLightIntensitySpeed);
        }

        audioSource.Play();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnEnable()
    {       
        StartCoroutine(TryFlickerLight());
    }
}
