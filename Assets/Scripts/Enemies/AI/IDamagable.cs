using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    public event Action OnDeath;
    public void TakeDamage(int damage);
}
