using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class BossFightVolume : MonoBehaviour
{
    private Volume volume;

    private float timeToIncreaseWeight = 150;
    private float timeToDecreaseWeight = 12;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<Volume>();
        GameManager.Instance.OnBossFightStarted += Instance_OnBossFightStarted;
        MageBoss.OnFightFinished += MageBoss_OnFightFinished;
    }

    private void Instance_OnBossFightStarted()
    {
        StartCoroutine(IncreaseWeight(timeToIncreaseWeight));
    }


    private void MageBoss_OnFightFinished()
    {
        StopAllCoroutines();
        StartCoroutine(DecreaseWeight(timeToDecreaseWeight));
    }

    private IEnumerator IncreaseWeight(float timeInSeconds)
    {
        while(volume.weight < 1)
        {
            volume.weight += Time.deltaTime / timeInSeconds;
            yield return null;
        }
    }

    private IEnumerator DecreaseWeight(float timeInSeconds)
    {
        while (volume.weight > 0)
        {
            volume.weight -= Time.deltaTime / timeInSeconds;
            yield return null;
        }
        volume.weight = 0;
    }
}
