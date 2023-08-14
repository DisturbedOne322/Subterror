using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnItemSelectedInGameScene : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private const string PLAY_ANIM_ANIM_TRIGGER = "OnPlay";
    private const string STOP_ANIM_ANIM_TRIGGER = "OnStop";

    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip audioClip;

    public void OnPointerEnter()
    {
        animator.SetTrigger(PLAY_ANIM_ANIM_TRIGGER);
        audioSource.PlayOneShot(audioClip);
    }

    public void OnPointerExit()
    {
        animator.SetTrigger(STOP_ANIM_ANIM_TRIGGER);
    }
}
