using UnityEngine;

public class TentacleIdleState : TentaclesBaseState
{
    private const string PULL_SPEED = "SpeedRate";

    private const string IDLE_ANIM = "Base Layer.Tentacle_Idle";

    private float normalSpeed = 1f;

    public override void EnterState(TentacleStateManager manager)
    {
        manager.animator.SetFloat(PULL_SPEED, normalSpeed);
        manager.animator.Play(IDLE_ANIM, -1, 0);
        manager.SetIdleSound();
    }
    public override void UpdateState(TentacleStateManager manager)
    {
        if(manager.underLight)
        {
            manager.SwitchState(manager.shrinkState);
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
