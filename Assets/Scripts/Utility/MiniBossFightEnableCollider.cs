using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBossFightEnableCollider : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;
    // Start is called before the first frame update
    void Start()
    {
        elevator.OnDeparted += Elevator_OnDeparted;
    }

    private void Elevator_OnDeparted()
    {
        GetComponent<BoxCollider2D>().enabled = true;
    }
}
