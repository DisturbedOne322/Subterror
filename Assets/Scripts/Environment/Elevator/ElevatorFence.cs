using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFence : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;

    [SerializeField]
    private Terminal terminal;

    [SerializeField]
    private Transform loweredPosition;

    [SerializeField]
    private Transform raisedPosition;

    private float lerpTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        elevator.OnArrived += Elevator_OnArrived;
        elevator.OnDeparted += Elevator_OnDeparted;
    }

    private void Elevator_OnDeparted()
    {
        StopAllCoroutines();
        StartCoroutine(RaiseFence());
    }

    private void Elevator_OnArrived()
    {
        if (!terminal.ElevatorArrived)
            return;
        StopAllCoroutines();
        StartCoroutine(LowerFence());
    }

    private IEnumerator LowerFence()
    {
        while (Vector2.Distance(transform.position, loweredPosition.position) > 0.1f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.Lerp(transform.position.y, loweredPosition.position.y, loweredPosition.position.y / transform.position.y * Time.deltaTime * lerpTime);
            transform.position = newPosition;

            yield return null;
        }
    }

    private IEnumerator RaiseFence()
    {
        yield return new WaitForSeconds(1.5f);
        while (Vector2.Distance(transform.position, raisedPosition.position) > 0.1f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.Lerp(transform.position.y, raisedPosition.position.y, transform.position.y/raisedPosition.position.y * Time.deltaTime * lerpTime);

            transform.position = newPosition;

            yield return null;
        }
    }
}
