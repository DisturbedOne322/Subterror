using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IParticleSpawnerCaller
{
    public event Action OnSpawnParticleAction;
}
