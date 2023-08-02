using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy: MonoBehaviour
{
    protected float moveSpeed;
    protected int healthPoints;
    protected int damage;
    protected float detectRange;
    protected float attackRange;

    protected bool hasDetectedPlayer;

    protected abstract void MoveToPlayer();
    protected abstract void AttackPlayer();
    protected abstract void DetectPlayer();
    public abstract void GetDamage(int damage);
}
