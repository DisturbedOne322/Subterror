using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    public event Action OnWeakPointBroken;
    private BoxCollider2D boxCollider;
    private ParticleSystem particleSys;
    int health;


    private void Awake()
    {
        health = 4;
    }


    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        particleSys = GetComponent<ParticleSystem>();
    }

    public void ResetWeakPoint()
    {
        boxCollider.enabled = true;
        particleSys.Play();
        health = 4;
    }

    public void GetDamage()
    {
        health -= 1;
        if (health <= 0)
        {
            boxCollider.enabled = false;
            OnWeakPointBroken?.Invoke();
            particleSys.Stop();  
        }
    }
}
