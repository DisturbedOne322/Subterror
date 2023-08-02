using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCheckpoint : MonoBehaviour
{
    public int nextMapPartUniqueID;
    public event Action<int> OnSpawnNextMapPart;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            OnSpawnNextMapPart?.Invoke(nextMapPartUniqueID);
        }
    }
}
