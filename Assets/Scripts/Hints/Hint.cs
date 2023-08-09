using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

[RequireComponent(typeof(IsPlayerInRange))]
public class Hint : MonoBehaviour
{
    public static event Action<HintSO> OnDisplayHint;
    public static event Action OnPlayerNotInRange;
    [SerializeField]
    private HintSO hintSO;

    private IsPlayerInRange isPlayerInRange;

    private bool playerInRange = false;
    private bool interactedRecently = false;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInRange = GetComponent<IsPlayerInRange>();
        isPlayerInRange.OnPlayerInRange += PlayerInRange_OnPlayerInRange;

        InputManager.Instance.OnInteract += Instance_OnInteract;
    }

    private void OnDestroy()
    {
        isPlayerInRange.OnPlayerInRange -= PlayerInRange_OnPlayerInRange;

        InputManager.Instance.OnInteract -= Instance_OnInteract;
    }

    private void Instance_OnInteract()
    {
        if (playerInRange)
        {
            OnDisplayHint?.Invoke(hintSO);
            interactedRecently = true;
        }
    }

    private void PlayerInRange_OnPlayerInRange(bool inRange)
    {
        playerInRange = inRange;
        if(interactedRecently)
        {
            if(!inRange)
            {
                interactedRecently = false;
                OnPlayerNotInRange?.Invoke();
            }
        }
    }
}
