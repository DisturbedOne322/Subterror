using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisableLightIfPlayerNotInRange : MonoBehaviour
{
    [SerializeField]
    private Light2D light;

    private IsPlayerInRange playerInRange;
    // Start is called before the first frame update
    void Start()
    {
        playerInRange = GetComponent<IsPlayerInRange>();
        playerInRange.OnPlayerInRange += PlayerInRange_OnPlayerInRange;
    }

    private void PlayerInRange_OnPlayerInRange(bool inRange)
    {
        light.enabled = inRange;
    }
}
