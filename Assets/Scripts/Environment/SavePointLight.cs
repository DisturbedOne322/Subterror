using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SavePointLight : MonoBehaviour
{
    private CheckCanSaveGame checkCanSaveGame;

    private Light2D light2D;

    private float defaultIntensity;
    private float offIntensity = 0;

    private bool lightsOn;

    private float smDampVelocity;
    private float smDampTime = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();    
        defaultIntensity = light2D.intensity;
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
        light2D.intensity = Mathf.SmoothDamp(light2D.intensity, targetIntensity, ref smDampVelocity, smDampTime);
    }
}
