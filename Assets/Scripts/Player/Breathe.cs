using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breathe : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement player;

    [SerializeField]
    private ParticleSystem breatheParticles;

    private float exhaleTimer = 3f;
    private float exhaleTimerTotal = 3f;

    private float inhaleTimer = 1f;
    private float inhaleTimerTotal = 3f;

    private bool highHP = true;

    private int gradientsForHPamount = 3;
    Gradient[] gradientsForHPValueArray;
    GradientColorKey[] highHPcolorKey;
    GradientAlphaKey[] highHPalphaKey;
    GradientColorKey[] midHPcolorKey;
    GradientAlphaKey[] midHPalphaKey;
    GradientColorKey[] lowHPcolorKey;
    GradientAlphaKey[] lowHPalphaKey;

    [SerializeField]
    private OptionsSO options;

    private void Start()
    {
        player.GetComponent<PlayerHealth>().OnHealthChanged += Player_OnHealthChanged;
        #region Instantiating gradients for HP values
        gradientsForHPValueArray = new Gradient[gradientsForHPamount];


        //high HP
        Gradient highHPGradient = new Gradient();
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        highHPcolorKey = new GradientColorKey[2];
        highHPcolorKey[0].color = Color.white;
        highHPcolorKey[0].time = 0.0f;
        highHPcolorKey[1].color = Color.white;
        highHPcolorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        highHPalphaKey = new GradientAlphaKey[2];
        highHPalphaKey[0].alpha = 0.33f;
        highHPalphaKey[0].time = 0.0f;
        highHPalphaKey[1].alpha = 0.2f;
        highHPalphaKey[1].time = 1.0f;

        highHPGradient.SetKeys(highHPcolorKey, highHPalphaKey);
        gradientsForHPValueArray[0] = highHPGradient;

        //moderate HP
        Gradient midHPGradient = new Gradient();
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        midHPcolorKey = new GradientColorKey[2];
        midHPcolorKey[0].color = Color.red;
        midHPcolorKey[0].time = 0.0f;
        midHPcolorKey[1].color = Color.white;
        midHPcolorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        midHPalphaKey = new GradientAlphaKey[2];
        midHPalphaKey[0].alpha = 0.33f;
        midHPalphaKey[0].time = 0.0f;
        midHPalphaKey[1].alpha = 0.2f;
        midHPalphaKey[1].time = 1.0f;

        midHPGradient.SetKeys(midHPcolorKey, midHPalphaKey);
        gradientsForHPValueArray[1] = midHPGradient;

        //low HP
        Gradient lowHPGradient = new Gradient();
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        lowHPcolorKey = new GradientColorKey[2];
        lowHPcolorKey[0].color = Color.red;
        lowHPcolorKey[0].time = 0.0f;
        lowHPcolorKey[1].color = Color.red;
        lowHPcolorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        lowHPalphaKey = new GradientAlphaKey[2];
        lowHPalphaKey[0].alpha = 0.33f;
        lowHPalphaKey[0].time = 0.0f;
        lowHPalphaKey[1].alpha = 0.2f;
        lowHPalphaKey[1].time = 1.0f;

        lowHPGradient.SetKeys(lowHPcolorKey, lowHPalphaKey);
        gradientsForHPValueArray[2] = lowHPGradient;
        #endregion
    }

    private void OnDestroy()
    {
        player.GetComponent<PlayerHealth>().OnHealthChanged -= Player_OnHealthChanged;

    }

    private void Player_OnHealthChanged(GameManager.PlayerHealthStatus playerHealthStatus)
    {
        var brearheGradient = breatheParticles.colorOverLifetime;

        switch (playerHealthStatus)
        {
            case GameManager.PlayerHealthStatus.HighHP:
                brearheGradient.color = gradientsForHPValueArray[0];
                highHP = true;
                break;
            case GameManager.PlayerHealthStatus.MidHP:
                brearheGradient.color = gradientsForHPValueArray[1];
                highHP = true;
                break;
            case GameManager.PlayerHealthStatus.LowHP:
                brearheGradient.color = gradientsForHPValueArray[2];
                highHP = false;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!options.breatingEnabled)
            return;

        exhaleTimer -= Time.deltaTime;
        inhaleTimer -= Time.deltaTime;

        if(inhaleTimer <= 0)
        {
            inhaleTimer = highHP ? inhaleTimerTotal : inhaleTimerTotal - 1;
            SoundManager.Instance.PlayInhaleSound();
        }

        if(exhaleTimer <= 0)
        {
            breatheParticles.Play();
            var velocityOverLifetime = breatheParticles.velocityOverLifetime;
            velocityOverLifetime.x = player.transform.localScale.x > 0 ? 4 : -4;

            var forceOverLifetime = breatheParticles.forceOverLifetime;
            forceOverLifetime.x = player.transform.localScale.x > 0 ? 5 : -5;

            SoundManager.Instance.PlayExhaleSound();

            exhaleTimer = highHP? exhaleTimerTotal : exhaleTimerTotal - 1;
        }
    }
}
