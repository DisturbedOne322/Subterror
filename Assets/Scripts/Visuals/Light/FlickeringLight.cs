using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(AudioSource)), RequireComponent(typeof(DynamicVolume))]
public class FlickeringLight : MonoBehaviour
{
    public event Action OnFlicker;
    public event Action OnRestore;

    private float flickerChance = 0.25f;
    private float flickerTryDelay = 10f;
    private float restoreLightIntensitySpeed = 0.05f;

    private Light2D light2D;

    private float delayBetweenRestoreLight = 5;

    // Start is called before the first frame update
    void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    IEnumerator TryFlickerLight()
    {
        while (true)
        {
            if (UnityEngine.Random.Range(0, 1f) < flickerChance)
            {
                OnFlicker?.Invoke();
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
        OnRestore?.Invoke();
        for (float i = 0; i <= 1; i += restoreLightIntensitySpeed)
        {
            light2D.intensity = i;
            yield return new WaitForSeconds(restoreLightIntensitySpeed);
        }
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
