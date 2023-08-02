using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerMiniBossFightAreaTrigger : MonoBehaviour
{
    public event Action<Transform[]> OnPlayerStartedBossFight;

    private Transform[] spawnPositions;

    // Start is called before the first frame update
    void Start()
    {
        spawnPositions = GetComponentsInChildren<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerStartedBossFight?.Invoke(spawnPositions);
            GetComponent<BoxCollider2D>().enabled = false;
            gameObject.SetActive(false);
        }
    }
}
