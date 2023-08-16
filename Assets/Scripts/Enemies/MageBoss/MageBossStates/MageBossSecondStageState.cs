using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBossSecondStageState : MageBossBaseState
{
    public override event System.Action<int, int> OnCoreDestroyed;
    public override event Action OnFightFinished;

    private int health = 8;

    private Animator animator;

    private float currentAttackCD = 4f;
    private float cdBetweenAttacks = 2f;

    private const string FLAMEBALL_ATTACK = "Flameball";
    private const string LASER_ATTACK = "Laser";
    private string[] attackSet = new string[2];


    private float switchStateDelay = 7f;

    //flameball
    private float spawnCDTotal = 0.5f; // cd between each flameball
    private float cdBetweenWaves = 0.5f;
    private int waveNumberTotal = 3;
    private int spawnAmountTotal = 5;
    private float fallSpeed = 13;
    private float scale = 1.2f;

    //laser
    private float laserAnimationDuration = 10;
    private float laserThickness = 0.6f;

    private bool defeated = false;

    private enum State
    {
        Idle,
        FlameballCast,
        LaserCast,
        LaserPrepare
    }

    private State state;

    public override void EnterState(MageBoss manager, string lastAttack)
    {
        manager.ResetColliders();
        manager.EnableSecondStageArms();
        animator = manager.GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0);
        animator.Play(MageBoss.APPEAR_ANIM);

        attackSet[0] = FLAMEBALL_ATTACK;
        attackSet[1] = LASER_ATTACK;
        health = manager.collidersArray.Length;
        manager.flameballspawnManager.OnAttackFinished += FlameballspawnManager_OnAttackFinished;
        manager.laser.OnAttackFinished += Laser_OnAttackFinished;

        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }

        state = State.Idle;
        LastAttack = lastAttack;
    }

    private void Laser_OnAttackFinished(MageBoss manager)
    {
        state = State.Idle;
        SetCDBetweenAttacks();
        animator.SetTrigger(MageBoss.LASER_END_TRIGGER);
    }

    private void FlameballspawnManager_OnAttackFinished()
    {
        state = State.Idle;
        SetCDBetweenAttacks();
    }

    private void MageBossFirstStageState_OnWeakPointBroken()
    {
        health--;
        OnCoreDestroyed?.Invoke(health, 8);
        if (health <= 0)
        {
            animator.SetTrigger(MageBoss.DEFEAT_ANIM_BOOL);
            defeated = true;
        }
        //Update UI
    }

    public override void UpdateState(MageBoss manager)
    {
        if (defeated)
        {
            switchStateDelay -= Time.deltaTime;
            if (switchStateDelay <= 0)
            {
                for (int i = 0; i < manager.collidersArray.Length; i++)
                {
                    manager.collidersArray[i].OnWeakPointBroken -= MageBossFirstStageState_OnWeakPointBroken;
                }
                manager.flameballspawnManager.OnAttackFinished -= FlameballspawnManager_OnAttackFinished;
                manager.laser.OnAttackFinished -= Laser_OnAttackFinished;
                manager.SwitchState(manager.thirdStageState);
            }
            return;
        }
        currentAttackCD -= Time.deltaTime;
        if (currentAttackCD < 0 && state == State.Idle)
        {
            switch (GetRandomAttack())
            {
                case FLAMEBALL_ATTACK:
                    FlameballCast(manager);
                    break;
                case LASER_ATTACK:
                    LaserCast(manager);
                    break;
            }
        }
        if (state == State.LaserPrepare)
        {
            AnimatorClipInfo[] m_CurrentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
            if (m_CurrentClipInfo.Length != 0)
            {
                if (m_CurrentClipInfo[0].clip.name == "LaserCast")
                {
                    manager.laser.InitializeLaser(laserAnimationDuration, laserThickness, manager);
                    state = State.LaserCast;
                }
            }
        }
    }

    private void SetCDBetweenAttacks()
    {
        currentAttackCD = cdBetweenAttacks;
    }

    private string GetRandomAttack()
    {
        int index = -1;
        do
        {
            index = UnityEngine.Random.Range(0, attackSet.Length);
        } while (attackSet[index] == LastAttack);
        return attackSet[index];
    }

    private void FlameballCast(MageBoss manager)
    {
        manager.PlayFlyUpSound();
        manager.flameballspawnManager.InitializeFlameballAttackProperties(waveNumberTotal, spawnAmountTotal, spawnCDTotal, cdBetweenWaves, fallSpeed, scale, true, new Vector3(-1, 0, 0));
        manager.animator.Play(MageBoss.FLAMEBALL_ANIM);
        state = State.FlameballCast;
        LastAttack = FLAMEBALL_ATTACK;
    }

    private void LaserCast(MageBoss manager)
    {
        manager.PlayFlyUpSound();
        manager.animator.Play(MageBoss.LASER_PREPARE_ANIM);
        state = State.LaserPrepare;
        LastAttack = LASER_ATTACK;
    }
}
