using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Terminal : MonoBehaviour
{
    private IsPlayerInRange isPlayerInRange;

    public event Action OnCallElevator;

    [SerializeField]
    private Elevator elevator;

    [SerializeField]
    private Light2D light2D;

    private bool playerInRange;

    private bool calledElevator = false;

    private bool elevatorArrived = false;
    public bool ElevatorArrived
    {
        get { return elevatorArrived; }
    }

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip buttonPress;
    [SerializeField]
    private AudioClip buttonError;

    private float originalDesiredDistance; 

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        isPlayerInRange = GetComponent<IsPlayerInRange>();
    }
    private void Start()
    {
        originalDesiredDistance = isPlayerInRange.desiredDistance;
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;
        InputManager.Instance.OnInteract += Instance_OnInteract;
        elevator.OnArrived += Elevator_OnArrived;
    }

    private void OnDestroy()
    {
        isPlayerInRange.OnPlayerInRange -= IsPlayerInRange_OnPlayerInRange;
        InputManager.Instance.OnInteract -= Instance_OnInteract;
        elevator.OnArrived -= Elevator_OnArrived;
    }

    private void Elevator_OnArrived()
    {
        light2D.intensity = 2;
        calledElevator = false;
        isPlayerInRange.desiredDistance = originalDesiredDistance;
    }

    private void Instance_OnInteract()
    {
        if (elevator.onWay)
        {
            TryPlayErrorSound();
            return;
        }

        if (elevatorArrived)
        {
            TryPlayErrorSound();
            return;
        }

        if (calledElevator)
        {
            TryPlayErrorSound();
            return;
        }
            

        if(playerInRange)
        {
            audioSource.PlayOneShot(buttonPress);
            calledElevator = true;
            light2D.intensity = 5;
            OnCallElevator?.Invoke();
            isPlayerInRange.desiredDistance = -1;
        }
    }

    private void TryPlayErrorSound()
    {
        if (!playerInRange)
            return;
        if (canPlayErrorSound)
        {
            audioSource.PlayOneShot(buttonError);
            StartCoroutine(ButtonSoundCD());
        }
    }

    private bool canPlayErrorSound = true;
    private IEnumerator ButtonSoundCD()
    {
        canPlayErrorSound = false;
        yield return new WaitForSeconds(2);
        canPlayErrorSound = true;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        playerInRange = obj;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Elevator"))
        {
            elevatorArrived = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Elevator"))
        {
            elevatorArrived = false;
        }
    }
}
