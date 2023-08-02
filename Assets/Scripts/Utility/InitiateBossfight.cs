using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateBossfight : MonoBehaviour
{
    public static event Action OnBossFightInitiated;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnBossFightInitiated?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
