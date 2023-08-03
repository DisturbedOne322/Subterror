using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ExecutionerHealth : MonoBehaviour, IDamagable
{
    private int healthPoint;

    [SerializeField]
    private int maxHealth;

    private BoxCollider2D boxCollider2D;

    private bool underLight = false;

    public void TakeDamage(int damage)
    {
        if(underLight)
        {
            if (healthPoint <= 0)
                return;

            healthPoint -= damage;
            if (healthPoint <= 0)
            {
                boxCollider2D.enabled = false;
                OnDeath?.Invoke();
            }
        }
    }

    public event Action OnDeath;

    private void OnEnable()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        boxCollider2D.enabled = true;
        healthPoint = maxHealth;
    }

    public void SetHealthTo1()
    {
        healthPoint = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        ExecutionerVisuals executionerVisuals = GetComponent<ExecutionerVisuals>();
        executionerVisuals.OnLighten += ExecutionerVisuals_OnLighten;
    }

    private void ExecutionerVisuals_OnLighten(bool obj)
    {
        underLight = obj;
    }
}
