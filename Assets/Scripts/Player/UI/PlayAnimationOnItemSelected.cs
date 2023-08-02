using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationOnItemSelected : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private const string PLAY_ANIM_ANIM_TRIGGER = "OnPlay";
    private const string STOP_ANIM_ANIM_TRIGGER = "OnStop";

    [SerializeField]
    private MainMenuSoundManager.SoundType type;

    public void OnPointerEnter()
    {
        animator.SetTrigger(PLAY_ANIM_ANIM_TRIGGER);
        if(type != MainMenuSoundManager.SoundType.NoSound)
            MainMenuSoundManager.Instance.PlayAnimSound(type);
    }

    public void OnPointerExit()
    {
        if (!MainMenuManager.playerActed)
            animator.SetTrigger(STOP_ANIM_ANIM_TRIGGER);
    }
}
