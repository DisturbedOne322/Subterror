using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoints : MonoBehaviour
{
    public event Action<int> OnSpawnNextAreaEnemies;
    public event Action<int> OnReduceNextAreaGlobalLight;
    public event Action<int> OnChangeBGMusic;
    public int checkpointID;

    private bool spawned = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (spawned)
                return;

            OnSpawnNextAreaEnemies?.Invoke(checkpointID);
            OnReduceNextAreaGlobalLight?.Invoke(checkpointID);
            OnChangeBGMusic?.Invoke(checkpointID);
            spawned = true;
        }
    }
}
