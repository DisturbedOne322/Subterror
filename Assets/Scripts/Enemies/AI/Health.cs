using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField]
    private int health;
    public int HealthPoints
    {
        get { return health; } 

    }

    private int maxHealth;
    public int MaxHealthPoint
    {
        get { return maxHealth; }
    }


    public event Action<int,int> OnHealthChange;

    public event Action OnDeath;
    private void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        if (health <= 0)
            return;

        health -= damage;
        OnHealthChange?.Invoke(health, maxHealth);
        if(health <= 0)
        {
            GetComponent<Rigidbody2D>().isKinematic = true;
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            OnDeath?.Invoke();
            Collider2D[] attachedColliders = GetComponentsInChildren<Collider2D>();
            for(int i = 0; i < attachedColliders.Length; i++)
                attachedColliders[i].enabled = false;
        }
    }

}
