using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Elevator : MonoBehaviour
{
    public event Action OnArrived;
    public event Action OnDeparted;

    private PlayerMovement player;
    private Vector3 lastPosition;


    [SerializeField]
    private Terminal terminalUpper;
    [SerializeField]
    private Terminal terminalButtom;

    [SerializeField]
    private Transform endPoint;

    [SerializeField]
    private Transform startPoint;

    private float elevatorSmDampVelocity;
    private readonly float elevatorSmoothTime = 6;

    private Vector3 destination;

    public bool onWay = false;

    [SerializeField]
    private EnemySpawnManager spawnManager;

    private ElevatorTerminal elevatorTerminal;

    // Start is called before the first frame update
    void Start()
    {
        terminalUpper.OnCallElevator += TerminalUpper_OnCallElevator;
        terminalButtom.OnCallElevator += TerminalButtom_OnCallElevator;
        player = GameManager.Instance.GetPlayerReference();

        if (spawnManager != null)
        {
            spawnManager.OnMiniBossFightStarted += SpawnManager_OnBossFightStarted;
            spawnManager.OnBossFightFinished += SpawnManager_OnBossFightFinished;
        }

        elevatorTerminal = GetComponentInChildren<ElevatorTerminal>();
        elevatorTerminal.OnInteract += ElevatorTerminal_OnInteract;

        lastPosition = transform.position;
    }

    private void OnDestroy()
    {
        if(elevatorTerminal != null)
        {
            elevatorTerminal.OnInteract -= ElevatorTerminal_OnInteract;
            terminalUpper.OnCallElevator -= TerminalUpper_OnCallElevator;
            terminalButtom.OnCallElevator -= TerminalButtom_OnCallElevator;
        }

        if (spawnManager != null)
        {
            spawnManager.OnMiniBossFightStarted -= SpawnManager_OnBossFightStarted;
            spawnManager.OnBossFightFinished -= SpawnManager_OnBossFightFinished;
        }
    }

    private void ElevatorTerminal_OnInteract()
    {
        OnDeparted?.Invoke();
        onWay = true;
        destination = Vector2.Distance(transform.position, startPoint.position)
            > Vector2.Distance(transform.position, endPoint.position) ?
            startPoint.position : endPoint.position;

        StartCoroutine(CallElevator(destination));
        player.transform.parent = this.transform;
    }

    private void SpawnManager_OnBossFightStarted()
    {
        StopAllCoroutines();
    }

    private void SpawnManager_OnBossFightFinished()
    {
        StartCoroutine(CallElevator(startPoint.position));
    }

    private void TerminalButtom_OnCallElevator()
    {
        StopAllCoroutines();
        StartCoroutine(CallElevator(endPoint.position));
        OnDeparted?.Invoke();
    }

    private void TerminalUpper_OnCallElevator()
    {
        StopAllCoroutines();
        StartCoroutine(CallElevator(startPoint.position));
        OnDeparted?.Invoke();
    }

    private IEnumerator CallElevator(Vector3 destination)
    {
        yield return new WaitForSeconds(4f);
        while (Vector2.Distance(transform.position, destination) > 5f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.SmoothDamp(transform.position.y, destination.y, ref elevatorSmDampVelocity, elevatorSmoothTime);
            transform.position = newPosition;

            yield return null;
        }
        player.transform.parent = null;
        OnArrived?.Invoke();
        onWay = false;
    }
}
