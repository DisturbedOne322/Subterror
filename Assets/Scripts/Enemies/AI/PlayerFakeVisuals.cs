using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFakeVisuals : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ApproachPlayer approachPlayer;

    private IDamagable damagable;

    private const string RUNNING_ANIM_BOOL = "IsRunning";
    private const string AIMING_ANIM_BOOL = "IsAiming";
    private const string DEATH_ANIM_TRIGGER = "OnDeath";

    // Start is called before the first frame update
    void Start()
    {
        approachPlayer.OnPlayerInRange += ApproachPlayer_OnPlayerInRange;
        damagable = GetComponent<IDamagable>();
        damagable.OnDeath += Damagable_OnDeath;
    }

    private void OnDestroy()
    {
        damagable.OnDeath -= Damagable_OnDeath;
        approachPlayer.OnPlayerInRange -= ApproachPlayer_OnPlayerInRange;
    }

    private void Damagable_OnDeath()
    {
        animator.SetTrigger(DEATH_ANIM_TRIGGER);
    }

    private void ApproachPlayer_OnPlayerInRange(bool inRange)
    {
        animator.SetBool(RUNNING_ANIM_BOOL, !inRange);
        animator.SetBool(AIMING_ANIM_BOOL, inRange);
    }
}
