using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DynamicSoundVolume
{
    public static float GetDynamicVolume(float maxDistance, float currentDistance)
    {
        return Mathf.Clamp(1 - currentDistance / maxDistance, 0, 1);
    }
}
