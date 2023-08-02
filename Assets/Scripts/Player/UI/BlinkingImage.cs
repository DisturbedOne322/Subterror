using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BlinkingImage : MonoBehaviour
{
    private Image image;

    [SerializeField, Range(0, 1f)]
    private float maxAlpha;

    [SerializeField, Range(0, 1f)]
    private float minAlpha;

    [SerializeField, Range(0, 1f)]
    private float step;

    [SerializeField, Min(0.01f)]
    private float animationTime;

    private float stepTime;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        stepTime = step * animationTime * (maxAlpha / minAlpha);
        stepTimeDelay = new WaitForSeconds(stepTime);
        if (maxAlpha < minAlpha)
        {
            this.enabled = false;
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
        stepTime = step * animationTime * (maxAlpha / minAlpha);
        if (maxAlpha < minAlpha)
        {
            this.enabled = false;
        }
    }

    private WaitForSeconds stepTimeDelay;

    private IEnumerator IncreaseAlpha()
    {
        float currentAlpha = image.color.a;
        for (; currentAlpha < maxAlpha; currentAlpha += step) 
        {
            Color temp = image.color;
            temp.a = currentAlpha;

            image.color = temp;

            yield return stepTimeDelay;
        }

        StartCoroutine(ReduceAlpha());
    }

    private IEnumerator ReduceAlpha()
    {
        float currentAlpha = image.color.a;
        for (; currentAlpha > minAlpha; currentAlpha -= step)
        {
            Color temp = image.color;
            temp.a = currentAlpha;

            image.color = temp;

            yield return stepTimeDelay;
        }
        StartCoroutine (IncreaseAlpha());
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(ReduceAlpha());
    }
}
