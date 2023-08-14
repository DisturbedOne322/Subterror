using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReduceHeadlightLightIntensityOnDeath : MonoBehaviour
{
    [SerializeField]
    private Light2D light2D;
    [SerializeField]
    private Light2D focusedLight2D;

    private IDamagable health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponentInParent<IDamagable>();
        health.OnDeath += Health_OnDeath;
    }

    private void OnDestroy()
    {
        health.OnDeath -= Health_OnDeath;
    }

    private void Health_OnDeath()
    {
        StartCoroutine(ReduceLightIntensity());  
    }

    private IEnumerator ReduceLightIntensity()
    {
        while (focusedLight2D.intensity > 0)
        {
            light2D.intensity -= Time.deltaTime;
            if (focusedLight2D != null)
            {
                focusedLight2D.intensity -= Time.deltaTime;
            }
            yield return null;
        }
    }
}
