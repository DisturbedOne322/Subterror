using UnityEngine;

public abstract class TentaclesBaseState
{
    public abstract void EnterState(TentacleStateManager manager);
    public abstract void UpdateState(TentacleStateManager manager);
    public abstract void OnCollisionEnter(TentacleStateManager manager, Collider2D collision);
}
