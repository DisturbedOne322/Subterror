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

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Elevator>().OnArrived += ElevatorLights_OnArrived;
        GetComponent<Elevator>().OnDeparted += ElevatorLights_OnDeparted;
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
