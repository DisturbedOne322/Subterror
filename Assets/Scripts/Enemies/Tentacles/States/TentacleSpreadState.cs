using UnityEngine;

public class TentacleSpreadState : TentaclesBaseState
{
    private const string SPREAD_ANIM = "Base Layer.Tentacle_Spread";
    
    public override void EnterState(TentacleStateManager manager)
    {
        if (manager.GetPrevState() is TentacleShrinkState)
        {
            var a = manager.animator.GetCurrentAnimatorStateInfo(0);
            manager.animator.Play(SPREAD_ANIM, -1, 1 - a.normalizedTime);
            return;
        }
         
        manager.animator.Play(SPREAD_ANIM, -1, 0);

        manager.SetShrinkSpreadSound();

    }
    public override void UpdateState(TentacleStateManager manager)
    {
        if (manager.underLight)
        {
            manager.SwitchState(manager.shrinkState);
        }
        if(manager.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
        {
            manager.SwitchState(manager.idleState);
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
