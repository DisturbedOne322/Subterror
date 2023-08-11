using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloor : MonoBehaviour
{
    private Elevator elevator;

    private void Start()
    {
        elevator = GetComponentInParent<Elevator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (elevator.onWay)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameManager.Instance.GetPlayerReference().GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            }
        }
    }
}
