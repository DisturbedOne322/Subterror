using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public event Action<GameManager.PlayerHealthStatus> OnHealthChanged;

    private int healthPoints = 10;
    private int maxHealthPoints = 10;

    public int HealthPoint
    {
        get { return healthPoints; }
    }

    public int MaxHealthPoint
    {
        get { return maxHealthPoints; }
    }

    public event Action OnDeath;

    public void RestoreFullHealth()
    {
        healthPoints = MaxHealthPoint;
        OnHealthChanged?.Invoke(GameManager.PlayerHealthStatus.HighHP);
    }

    public void TakeDamage(int damage)
    {
        if (healthPoints <= 0)
            return;

        healthPoints -= damage;
        OnHealthChanged?.Invoke(CalculateHealthStatus());
        if (healthPoints <= 0)
        {
            //OnPlayerDied?.Invoke();
            OnDeath?.Invoke();
            SoundManager.Instance.PlayDeathSound();
        }
        SoundManager.Instance.PlayGetHurtSound();
    }

    public void GetCriticaldamage()
    {
        if (healthPoints > 3)
            healthPoints -= 3;
        else
            healthPoints = 1;
        OnHealthChanged?.Invoke(CalculateHealthStatus());
        SoundManager.Instance.PlayGetHurtSound();
    }

    private GameManager.PlayerHealthStatus CalculateHealthStatus()
    {
        return healthPoints * 1.0f / maxHealthPoints > 0.5f ? GameManager.PlayerHealthStatus.HighHP :
            healthPoints * 1.0f / maxHealthPoints > 0.25f ? GameManager.PlayerHealthStatus.MidHP : GameManager.PlayerHealthStatus.LowHP;
    }

    private void Start()
    {
        QTE.instance.OnQTERoundFailed += Instance_OnQTERoundFailed;
    }

    private void OnDestroy()
    {
        QTE.instance.OnQTERoundFailed -= Instance_OnQTERoundFailed;
    }

    private void Instance_OnQTERoundFailed(int obj)
    {
        TakeDamage(obj);
    }

}
