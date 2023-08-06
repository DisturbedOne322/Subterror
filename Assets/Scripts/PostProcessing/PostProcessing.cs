using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using static Unity.Burst.Intrinsics.X86.Avx;

public class PostProcessing : MonoBehaviour
{
    [SerializeField]
    private Volume volume;
    private UnityEngine.Rendering.Universal.Vignette vig;
    private PlayerHealth playerHealth;


    private bool playerAtLowHP = false;
    private float lowerBoundVigIntensity = 0.2f;
    private float higherBoundVigIntensity = 0.4f;
    private float intensityModifier = 0.5f;

    private float vigChangeSpeedModifier = 0.7f;
    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameManager.Instance.GetPlayerReference().GetComponent<PlayerHealth>();
        playerHealth.OnHealthChanged += Player_OnHealthChanged;
        UnityEngine.Rendering.Universal.Vignette temp;
        if (volume.profile.TryGet<UnityEngine.Rendering.Universal.Vignette>(out temp))
        {
            vig = temp;
        }
    }

    private void OnDestroy()
    {
        playerHealth.OnHealthChanged -= Player_OnHealthChanged;
    }

    private void Player_OnHealthChanged(GameManager.PlayerHealthStatus healthStatus)
    {
        if (healthStatus == GameManager.PlayerHealthStatus.LowHP)
        {
            vig.active = true;
            playerAtLowHP = true;
        }
        else
        {
            vig.active = false;
            playerAtLowHP = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(playerAtLowHP)
        {
            if(vig.intensity.value > higherBoundVigIntensity)
            {
                intensityModifier *= -1;
            }
            else if(vig.intensity.value < lowerBoundVigIntensity)
            {
                intensityModifier *= -1;
            }
            vig.intensity.value += intensityModifier * Time.deltaTime * vigChangeSpeedModifier;
        }
    }
}
