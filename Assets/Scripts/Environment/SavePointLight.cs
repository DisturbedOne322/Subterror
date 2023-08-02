using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SavePointLight : MonoBehaviour
{
    private CheckCanSaveGame checkCanSaveGame;

    private Light2D light;

    private float defaultIntensity;
    private float offIntensity = 0;

    private bool lightsOn;

    private float smDampVelocity;
    private float smDampTime = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();    
        defaultIntensity = light.intensity;
        checkCanSaveGame = GetComponentInParent<CheckCanSaveGame>();
        checkCanSaveGame.CanSaveGame += CheckCanSaveGame_CanSaveGame;
    }

    private void CheckCanSaveGame_CanSaveGame(bool canSave)
    {
        lightsOn = canSave;
    }

    private void Update()
    {
        float targetIntensity = lightsOn? defaultIntensity : offIntensity;
        light.intensity = Mathf.SmoothDamp(light.intensity, targetIntensity, ref smDampVelocity, smDampTime);
    }
}
