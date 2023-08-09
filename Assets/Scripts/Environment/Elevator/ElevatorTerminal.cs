using System;
using UnityEngine;

public class ElevatorTerminal : MonoBehaviour
{
    public event Action OnInteract;
    private IsPlayerInRange isPlayerInRange;
    private Elevator elevator;

    private bool playerInRange = false;

    private bool departed = false;

    private float originalDistance;

    // Start is called before the first frame update
    void Start()
    {
        isPlayerInRange = GetComponent<IsPlayerInRange>();
        originalDistance = isPlayerInRange.desiredDistance;
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;
        elevator = GetComponentInParent<Elevator>();

        elevator.OnArrived += Elevator_OnArrived;

        InputManager.Instance.OnInteract += Instance_OnInteract;
    }

    private void OnDestroy()
    {
        isPlayerInRange.OnPlayerInRange -= IsPlayerInRange_OnPlayerInRange;
        elevator.OnArrived -= Elevator_OnArrived;
        InputManager.Instance.OnInteract -= Instance_OnInteract;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        playerInRange = obj;
    }

    private void Elevator_OnArrived()
    {
        departed = false;
        isPlayerInRange.desiredDistance = originalDistance;
    }

    private void Instance_OnInteract()
    {
        if(playerInRange && !departed)
        {
            OnInteract?.Invoke();
            isPlayerInRange.desiredDistance = -1;
            departed = true;
        }
    }
}
