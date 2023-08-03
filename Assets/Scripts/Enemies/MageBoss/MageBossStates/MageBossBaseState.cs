using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MageBossBaseState
{
    public abstract void EnterState(MageBoss manager, string lastAttack);
    public abstract void UpdateState(MageBoss manager);
    public abstract event Action<int,int> OnCoreDestroyed;
    public abstract event Action OnFightFinished;
    public string LastAttack;
}