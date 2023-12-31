using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    [SerializeField]
    private Light2D light2D;
    [SerializeField]
    private float maxLightIntensity = 5;
    private float minLightIntensity = 0;
    private float targetLightIntensity;

    private float changeTime = 0.5f;

    private float smDampVelocity;

    private void Awake()
    {
        targetLightIntensity = minLightIntensity;
    }

    private void Update()
    {
        light2D.intensity = Mathf.SmoothDamp(light2D.intensity, targetLightIntensity, ref smDampVelocity, changeTime);
    }

    public void OnPointerEnter()
    {
        targetLightIntensity = maxLightIntensity;
    }

    public void OnPointerExit()
    {
        targetLightIntensity = minLightIntensity;
    }
}
