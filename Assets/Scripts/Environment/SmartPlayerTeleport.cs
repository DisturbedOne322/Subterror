using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SmartPlayerTeleport : MonoBehaviour
{
    [SerializeField]
    private Transform teleportToDestination;
    private Transform playerPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerPos = collision.gameObject.GetComponent<Transform>();
            Invoke(nameof(TeleportPlayer),3);
        }
    }

    private void TeleportPlayer()
    {
        playerPos.position = teleportToDestination.position;
    }
}
