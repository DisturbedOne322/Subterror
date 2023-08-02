using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInClawAttackRange : MonoBehaviour
{
    public event Action OnPlayerInClawAttackRange;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            OnPlayerInClawAttackRange?.Invoke();
    }
}
