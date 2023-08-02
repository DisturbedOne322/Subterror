using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleDisappearState : TentaclesBaseState
{
    private const string PULL_SPEED = "SpeedRate";
    private const string SHRINK_ANIM = "Base Layer.Tentacle_Shrink";

    private float disappearSpeed = 0.25f;

    public override void EnterState(TentacleStateManager manager)
    {
        var a = manager.animator.GetCurrentAnimatorStateInfo(0);
        float animTimeNormalized = a.normalizedTime > 1 ? 0.99f : a.normalizedTime;
        manager.animator.Play(SHRINK_ANIM, -1, 1 - animTimeNormalized);
        manager.animator.SetFloat(PULL_SPEED, disappearSpeed);
    }
    public override void UpdateState(TentacleStateManager manager)
    {
    }
    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
    }
}
