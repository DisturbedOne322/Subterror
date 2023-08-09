using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReduceGlobalLight : MonoBehaviour
{
    private Light2D globalLight;
    private Checkpoints checkpoint;

    private float defaultIntensity = 0.4f;

    private float smDampVelocity;
    private float smDampTime = 10;

    // Start is called before the first frame update
    void Start()
    {
        globalLight = GameObject.FindGameObjectWithTag("GlobalLight").GetComponent<Light2D>();
        checkpoint = GetComponent<Checkpoints>();
        checkpoint.OnReduceNextAreaGlobalLight += Checkpoint_OnReduceNextAreaGlobalLight;
    }

    private void OnDestroy()
    {
        checkpoint.OnReduceNextAreaGlobalLight -= Checkpoint_OnReduceNextAreaGlobalLight;
    }

    private void Checkpoint_OnReduceNextAreaGlobalLight(int id)
    {
        StartCoroutine(ReduceGlobalLightIntensity(id));
    }

    private IEnumerator ReduceGlobalLightIntensity(int id)
    {
        float targetIntensity = defaultIntensity - (id / 50.0f);
        while(globalLight.intensity > targetIntensity)
        {
            globalLight.intensity = Mathf.SmoothDamp(globalLight.intensity, targetIntensity, ref smDampVelocity, smDampTime);
            yield return null;
        }
    }
}
