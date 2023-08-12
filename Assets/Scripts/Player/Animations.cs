using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animations : MonoBehaviour
{
    private const string RUNNING_ANIMATION = "IsRunning";
    private const string FALLING_ANIMATION = "IsFalling";
    //private const string JUMPING_ANIMATION = "HasJumped";
    private const string RELOAD_ANIMATION = "HasReloaded";
    private const string AIM_ANIMATION = "IsAiming";
    private const string DEATH_ANIM_TRIGGER = "OnDeath";
    private const string AIR_ANIM_BOOL = "InAir";
    private const string JUMP_ANIM_BOOL = "HasJumped";

    private float stepSoundTimer = 0.4f;
    private float stepSoundTimerTotal = 0.4f;

    private Animator animator;
    private PlayerMovement player;
    private PlayerHealth playerHealth;

    private bool playerInQTE = false;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        InputManager.Instance.OnFocusActionEnded += Instance_OnFocusActionEnded;
        InputManager.Instance.OnFocusActionStarted += Instance_OnFocusActionStarted;
        InputManager.Instance.OnJumpAction += Instance_OnJumpAction;
        Shoot.OnSuccessfulReload += Shoot_OnSuccessfulReload;

        QTE.instance.OnQTEStart += QTE_OnQTEStart;
        QTE.instance.OnQTEEnd += Instance_OnQTEEnd;

        playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.OnDeath += Player_OnPlayerDied;
        player.OnLanded += Player_OnLanded;
        player.OnInAir += Player_OnInAir;
        PlayerMovement.OnInCutscene += PlayerMovement_OnInCutscene;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnFocusActionEnded -= Instance_OnFocusActionEnded;
        InputManager.Instance.OnFocusActionStarted -= Instance_OnFocusActionStarted;
        InputManager.Instance.OnJumpAction -= Instance_OnJumpAction;
        Shoot.OnSuccessfulReload -= Shoot_OnSuccessfulReload;

        QTE.instance.OnQTEStart -= QTE_OnQTEStart;
        QTE.instance.OnQTEEnd -= Instance_OnQTEEnd;

        playerHealth.OnDeath -= Player_OnPlayerDied;
        player.OnLanded -= Player_OnLanded;
        player.OnInAir -= Player_OnInAir;
        PlayerMovement.OnInCutscene -= PlayerMovement_OnInCutscene;
    }

    private void Instance_OnJumpAction()
    {
        animator.SetBool(JUMP_ANIM_BOOL, true);
        StartCoroutine(ResetJumpAnim());
    }

    private IEnumerator ResetJumpAnim()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool(JUMP_ANIM_BOOL, false);
    }

    private void Player_OnInAir()
    {
        animator.SetBool(AIR_ANIM_BOOL, true);
    }
    private void Player_OnLanded()
    {
        animator.SetBool(AIR_ANIM_BOOL, false);
    }

    private void PlayerMovement_OnInCutscene()
    {
        animator.enabled = false;
        animator.enabled = true;
    }



    private void Player_OnPlayerDied()
    {
        animator.SetTrigger(DEATH_ANIM_TRIGGER);
    }

    private void Instance_OnQTEEnd(IQTECaller caller)
    {
        playerInQTE = false;
    }

    private void QTE_OnQTEStart()
    {
        playerInQTE = true;
    }

    private void Instance_OnFocusActionEnded()
    {
        animator.SetBool(AIM_ANIMATION, false);
    }

    private void Instance_OnFocusActionStarted()
    {
        animator.SetBool(AIM_ANIMATION, true);
    }

    private void Shoot_OnSuccessfulReload(int a)
    {
        animator.SetTrigger(RELOAD_ANIMATION);
    }

    //private void Instance_OnJumpAction()
    //{
    //    if(player.IsGrounded)
    //        animator.SetTrigger(JUMPING_ANIMATION);
    //}
    private void MagOutSoundEvent()
    {
        SoundManager.Instance.PlayMagOutSound();
    }
    private void MagInSoundEvent()
    {
        SoundManager.Instance.PlayMagInSound();
    }
    private void ReloadSoundEvent()
    {
        SoundManager.Instance.PlayReloadSound();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInQTE)
        {
            animator.SetBool(RUNNING_ANIMATION, false);
            return;
        }
        float movement = InputManager.Instance.GetMovementDirection();
        if(movement != 0) 
        {
            animator.SetBool(RUNNING_ANIMATION, true);
            if (player.IsGrounded)
            {
                stepSoundTimer -= Time.deltaTime;
                if (stepSoundTimer < 0)
                {
                    stepSoundTimer = stepSoundTimerTotal;
                    if(player.inMud)
                        SoundManager.Instance.PlayMudStepsSound();
                    else
                        SoundManager.Instance.PlayStepsSound();
                }
            }
        }
        else
        {
            animator.SetBool(RUNNING_ANIMATION, false);
            stepSoundTimer = stepSoundTimerTotal;
        }

        animator.SetBool(FALLING_ANIMATION, player.IsFalling);
    }
}
