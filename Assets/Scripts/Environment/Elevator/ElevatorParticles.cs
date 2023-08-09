using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Elevator))]
public class ElevatorParticles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] sparkParticles;

    private Elevator elevator;

    // Start is called before the first frame update
    void Start()
    {
        elevator = GetComponent<Elevator>();
        elevator.OnDeparted += ElevatorParticles_OnDeparted;
        elevator.OnArrived += ElevatorParticles_OnArrived;
    }

    private void OnDestroy()
    {
        if(elevator != null )
        {
            elevator.OnDeparted -= ElevatorParticles_OnDeparted;
            elevator.OnArrived -= ElevatorParticles_OnArrived;
        }
    }

    private void ElevatorParticles_OnArrived()
    {
        foreach (var particle in sparkParticles)
        {
            particle.Stop();
        }
    }

    private void ElevatorParticles_OnDeparted()
    {
        foreach (var particle in sparkParticles)
        {
            particle.Play();
        }
    }
}
