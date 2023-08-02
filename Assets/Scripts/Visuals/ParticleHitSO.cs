using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class ParticleHitSO : ScriptableObject
{
    public LayerMask[] layerMaskArray;
    public ParticleSystem[] particleArray;
}
