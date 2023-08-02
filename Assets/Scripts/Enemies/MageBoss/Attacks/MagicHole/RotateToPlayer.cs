using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToPlayer : MonoBehaviour
{
    PlayerMovement player;
    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
    }

    // Update is called once per frame
    void Update()
    {
        transform.right = -(player.transform.position - transform.position);
    }
}
