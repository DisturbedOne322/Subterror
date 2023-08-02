using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerTriggerArea : MonoBehaviour
{
    public event Action<Transform> OnPlayerEnteredTriggerArea;
    [SerializeField]
    private GameObject spawnPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            OnPlayerEnteredTriggerArea?.Invoke(spawnPos.transform);
            gameObject.SetActive(false);
        }
    }
}
