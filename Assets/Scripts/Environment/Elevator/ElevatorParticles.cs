using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Elevator))]
public class ElevatorParticles : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] sparkParticles;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Elevator>().OnDeparted += ElevatorParticles_OnDeparted;
        GetComponent<Elevator>().OnArrived += ElevatorParticles_OnArrived; ;

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
