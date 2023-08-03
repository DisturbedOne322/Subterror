using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerInRange : MonoBehaviour
{
    private PlayerMovement player;

    [SerializeField]
    public float desiredDistance;

    [SerializeField]
    private Transform point;

    public event Action<bool> OnPlayerInRange;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        player = GameManager.Instance.GetPlayerReference();
        StartCoroutine(EvaluatePlayerInRange());
    }

    private WaitForSeconds delay = new WaitForSeconds(0.01f);

    private IEnumerator EvaluatePlayerInRange()
    {
        while(true)
        {
            OnPlayerInRange?.Invoke(Vector2.Distance(point.position, player.transform.position) < desiredDistance);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
