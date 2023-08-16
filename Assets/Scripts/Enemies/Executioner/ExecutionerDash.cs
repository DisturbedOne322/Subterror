using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerDash : MonoBehaviour
{
    [SerializeField]
    private float dashDistance;

    private PlayerMovement player;

    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
    }

    public void Dash()
    {
        Vector2 direction = player.transform.position - transform.position;
        transform.Translate(direction.normalized * dashDistance);
    }
}
