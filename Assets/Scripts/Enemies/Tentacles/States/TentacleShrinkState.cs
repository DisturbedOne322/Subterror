using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleShrinkState : TentaclesBaseState
{
    private const string PULL_SPEED = "SpeedRate";
    private const string SHRINK_ANIM = "Base Layer.Tentacle_Shrink";

    private readonly float normalSpeed = 1f;
    private readonly float pullSpeed = 0.1f;

    
    public override void EnterState(TentacleStateManager manager)
    {
        manager.animator.SetFloat(PULL_SPEED, manager.pullingPlayer? pullSpeed : normalSpeed);
        manager.SetShrinkSpreadSound();

        if (manager.GetPrevState() is TentacleIdleState)
        {
            manager.animator.Play(SHRINK_ANIM, -1, 0);
            return;
        }

        var a = manager.animator.GetCurrentAnimatorStateInfo(0);
        float animTimeNormalized = a.normalizedTime > 1 ? 0.99f : a.normalizedTime;
        manager.animator.Play(SHRINK_ANIM, -1, 1 - animTimeNormalized);

    }

    public override void UpdateState(TentacleStateManager manager)
    {
        if(!manager.underLight && !manager.pullingPlayer)
        {
            manager.SwitchState(manager.spreadState);
        }
        if(manager.playerFreed)
        {
            manager.SwitchState(manager.disappearState);
        }

        if(manager.pullingPlayer)
        {
            manager.animator.SetFloat(PULL_SPEED, pullSpeed);
            var a = manager.animator.GetCurrentAnimatorStateInfo(0);
            float animTimeNormalized = a.normalizedTime > 1 ? 0.99f : a.normalizedTime;

            AnimatorClipInfo[] m_CurrentClipInfo = manager.animator.GetCurrentAnimatorClipInfo(0);
            if (m_CurrentClipInfo.Length != 0)
            {
                if(m_CurrentClipInfo[0].clip.name != "Tentacle_Shrink")
                    return;
            }

            if (animTimeNormalized >= 0.65f)
            {
                manager.pullingPlayer = false;
                manager.playerFreed = true;
                SoundManager.Instance.PlayBoneCrackSound();
                manager.playerHealth.GetCriticaldamage();
                QTE.instance.EndQTE();
            }
        }
    }
    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
    }
}
