using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostVisuals : MonoBehaviour
{
    private Ghost ghost;

    private Animator animator;

    private readonly string ATTACK_ANIM = "IsAttacking";
    private readonly string DETECTED_ANIM = "DetectedPlayer";
    private readonly string DISAPPEAR_ANIM = "AttackFinished";
    private readonly string GET_DAMAGED_ANIM = "IsAttacked";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ghost = GetComponent<Ghost>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //FocusedHeadlight.OnGhostFound += FocusedHeadlight_OnGhostFound;
        QTE.instance.OnQTEEnd += Instance_OnQTEEnd;
        ghost.OnAttack += Ghost_OnAttack;
        ghost.OnPlayerDetected += Ghost_OnPlayerDetected;
    }


    private void OnDestroy()
    {
        //FocusedHeadlight.OnGhostFound -= FocusedHeadlight_OnGhostFound;
        QTE.instance.OnQTEEnd -= Instance_OnQTEEnd;
        ghost.OnAttack -= Ghost_OnAttack;
        ghost.OnPlayerDetected -= Ghost_OnPlayerDetected;
        ghost.OnGhostSpotted += Ghost_OnGhostSpotted;
    }

    private void Ghost_OnGhostSpotted()
    {
        animator.SetBool(GET_DAMAGED_ANIM, true);
    }

    private void Ghost_OnPlayerDetected()
    {
        animator.SetBool(DETECTED_ANIM, true);
    }

    private void Ghost_OnAttack()
    {
        animator.SetBool(ATTACK_ANIM, true);
    }

    private void Instance_OnQTEEnd(IQTECaller caller)
    {
        if (caller == ghost)
        {
            animator.SetTrigger(DISAPPEAR_ANIM);
        }
    }

    private void FocusedHeadlight_OnGhostFound(Ghost ghost)
    {
        if (ghost != this.ghost)
            return;
        animator.SetBool(GET_DAMAGED_ANIM, true);
    }
    private void OnEnable()
    {
        animator.Rebind();
        animator.Update(0f);
    }

}
