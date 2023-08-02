using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportedVisuals : MonoBehaviour
{
    private Animator animator;
    private readonly string APPEAR_ANIM = "PlayerInRange";
    private readonly string DISAPPEAR_ANIM = "PlayerPulled";

    private Teleporter teleporter;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        teleporter = GetComponent<Teleporter>();
        teleporter.OnAppear += Teleporter_OnAppear;
        teleporter.OnDisappear += Teleporter_OnDisappear;
    }

    private void OnDestroy()
    {
        if (teleporter == null)
            return;
        teleporter.OnAppear -= Teleporter_OnAppear;
        teleporter.OnDisappear -= Teleporter_OnDisappear;
    }

    private void Teleporter_OnDisappear()
    {
        animator.SetTrigger(DISAPPEAR_ANIM);
    }

    private void Teleporter_OnAppear()
    {
        animator.SetTrigger(APPEAR_ANIM);
    }
}
