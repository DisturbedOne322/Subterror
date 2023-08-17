using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class TentacleAttackState : TentaclesBaseState
{
    private const string SPREAD_ANIM = "Base Layer.Tentacle_Spread";
    private const string PULL_SPEED = "SpeedRate";

    private bool attacked = false;
    private float attackSpeed = 5.5f;
    private readonly float raycastLength = 10f;
    RaycastHit2D raycastHit2D;


    public override void EnterState(TentacleStateManager manager)
    {
        manager.animator.Play(SPREAD_ANIM,-1, 0);
        manager.animator.SetFloat(PULL_SPEED, attackSpeed);
        manager.animator.enabled = false;

        manager.SetShrinkSpreadSound();

    }
    public override void UpdateState(TentacleStateManager manager)
    {
        raycastHit2D = Physics2D.Raycast(manager.transform.position, Vector2.down, raycastLength, manager.playerLayer);
        if (raycastHit2D && !attacked)
        {
            manager.animator.enabled = true;
            attacked = true;
            manager.PlayAttackSound();
        }
        if(attacked)
        {
            var a = manager.animator.GetCurrentAnimatorStateInfo(0);
            if (a.normalizedTime >= 0.95f)
            {
                manager.SwitchState(manager.disappearState);
            }
        }

    }
    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            QTE.instance.StartQTE(manager, QTE.QTE_TYPE.SmashingButtons);
            manager.SwitchState(manager.shrinkState);
            manager.pullingPlayer = true;
        }
    }
}
