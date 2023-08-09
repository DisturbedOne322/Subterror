using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Elevator))]
public class ElevatorLights : MonoBehaviour
{
    [SerializeField]
    private Light2D freeLight;

    [SerializeField]
    private Light2D occupiedLight;

    private float onIntensity = 30;
    private float offIntensity = 3;

    private Elevator elevator;

    // Start is called before the first frame update
    void Start()
    {
        elevator = GetComponent<Elevator>();
        elevator.OnArrived += ElevatorLights_OnArrived;
        elevator.OnDeparted += ElevatorLights_OnDeparted;
    }

    private void OnDestroy()
    {
        elevator.OnArrived -= ElevatorLights_OnArrived;
        elevator.OnDeparted -= ElevatorLights_OnDeparted;
    }

    private void ElevatorLights_OnDeparted()
    {
        occupiedLight.intensity = onIntensity;
        freeLight.intensity = offIntensity;
    }

    private void ElevatorLights_OnArrived()
    {
        occupiedLight.intensity = offIntensity;
        freeLight.intensity = onIntensity;
    }
}
