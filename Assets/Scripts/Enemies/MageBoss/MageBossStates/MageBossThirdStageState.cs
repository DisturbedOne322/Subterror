using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBossThirdStageState : MageBossBaseState
{
    public override event System.Action<int, int> OnCoreDestroyed;
    public override event System.Action OnFightFinished;

    private int health = 8;

    private Animator animator;

    private float currentAttackCD = 4f;
    private float cdBetweenAttacks = 3;

    private const string FLAMEBALL_ATTACK = "Flameball";
    private const string LASER_ATTACK = "Laser";
    private const string EXCALIBUR_ATTACK = "Excalibur";
    private string[] attackSet = new string[3];
    private List<string> attackSequence = new List<string>();

    private MageBoss manager;

    private float switchStateDelay = 7f;

    //flameball
    private float spawnCDTotal = 0.6f; // cd between each flameball
    private float cdBetweenWaves = 1.25f;
    private int waveNumberTotal = 3;
    private int spawnAmountTotal = 4;
    private float fallSpeed = 15;
    private float scale = 1.5f;

    //laser
    private float laserAnimationDuration = 12;
    private float laserThickness = 0.85f;

    //excalibur
    private float excaliburSpawnDelay = 3f;

    //magic hole
    private float magicHoleDuration = 5;
    private bool magicHoleOnLastAttack = false;

    private bool defeated = false;
    private bool finishedFight = false;

    private enum State
    {
        Idle,
        FlameballCast,
        LaserCast,
        LaserPrepare,
        ExcaliburCast
    }

    private State state;

    public override void EnterState(MageBoss manager, string lastAttack)
    {
        this.manager = manager;
        manager.ResetColliders();
        manager.EnableThirdStageArms();
        animator = manager.GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0);
        animator.Play(MageBoss.APPEAR_ANIM);

        attackSet[0] = FLAMEBALL_ATTACK;
        attackSet[1] = LASER_ATTACK;
        attackSet[2] = EXCALIBUR_ATTACK;
        health = manager.collidersArray.Length;
        manager.flameballspawnManager.OnAttackFinished += FlameballspawnManager_OnAttackFinished;
        manager.laser.OnAttackFinished += Laser_OnAttackFinished;

        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }

        state = State.Idle;
        LastAttack = lastAttack;

        manager.excaliburAttack.OnSwordAttackFinished += ExcaliburAttack_OnSwordRetured;
    }


    private void ExcaliburAttack_OnSwordRetured()
    {
        manager.SwordReturned();
        state = State.Idle;
        SetCDBetweenAttacks();
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
            animator.SetTrigger(MageBoss.FINISHED_FIGHT_TRIGGER);
            OnFightFinished?.Invoke();
            defeated = true;
        }
    }

    public override void UpdateState(MageBoss manager)
    {
        if (defeated)
        {
            if (finishedFight)
                return;
            manager.excaliburAttack.OnSwordAttackFinished -= ExcaliburAttack_OnSwordRetured;
            manager.flameballspawnManager.OnAttackFinished -= FlameballspawnManager_OnAttackFinished;
            manager.laser.OnAttackFinished -= Laser_OnAttackFinished;

            for (int i = 0; i < manager.collidersArray.Length; i++)
            {
                manager.collidersArray[i].OnWeakPointBroken -= MageBossFirstStageState_OnWeakPointBroken;
            }
            finishedFight = true;
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
                case EXCALIBUR_ATTACK:
                    ExcaliburCast(manager);
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
        //if among the last 4 attacks 1 attack type didn't occur, do this attack
        if(attackSequence.Count > 3)
        {
            for(int i = 0; i < attackSet.Length; i++)
            {
                if (!attackSequence.Contains(attackSet[i]))
                {
                    string attack = attackSet[i];
                    attackSequence.Clear();
                    attackSequence.Add(attack);
                    return attack;
                }
            }
            attackSequence.Clear();
        }


        int index = -1;
        do
        {
            index = UnityEngine.Random.Range(0, attackSet.Length);
        } while (attackSet[index] == LastAttack);
        attackSequence.Add(attackSet[index]);
        return attackSet[index];
    }

    private void MagicHoleCast(MageBoss manager)
    {
        manager.magicHole.Initialize(magicHoleDuration);
        manager.animator.SetTrigger(MageBoss.MAGIC_HOLE_ATTACK_TRIGGER);
    }

    private void ExcaliburCast(MageBoss manager)
    {
        manager.PlayThrowSwordSound();
        manager.StartCoroutine(SpawnExcalibur(manager));
        manager.animator.SetTrigger(MageBoss.EXCALIBUR_ATTACK_TRIGGER);
        state = State.ExcaliburCast;
        LastAttack = EXCALIBUR_ATTACK;

        CreateMagicHole(5);
    }

    IEnumerator SpawnExcalibur(MageBoss manager)
    {
        yield return new WaitForSeconds(excaliburSpawnDelay);

        manager.excaliburAttack.gameObject.SetActive(true);
        manager.excaliburAttack.Initialize(manager.swordSpawnPoint.position);
    }

    private void FlameballCast(MageBoss manager)
    {
        manager.PlayFlyUpSound();
        manager.flameballspawnManager.InitializeFlameballAttackProperties(waveNumberTotal, spawnAmountTotal, spawnCDTotal, cdBetweenWaves, fallSpeed, scale, true, new Vector3(-1, 0, 0));
        manager.animator.Play(MageBoss.FLAMEBALL_ANIM);
        state = State.FlameballCast;
        LastAttack = FLAMEBALL_ATTACK;

        CreateMagicHole(9);
    }

    private void LaserCast(MageBoss manager)
    {
        manager.PlayFlyUpSound();
        manager.animator.Play(MageBoss.LASER_PREPARE_ANIM);
        state = State.LaserPrepare;
        LastAttack = LASER_ATTACK;
        CreateMagicHole(10);
    }

    private void CreateMagicHole(float duration)
    {
        if(magicHoleOnLastAttack)
        {
            magicHoleOnLastAttack = false;
            return;
        }
        
        magicHoleDuration = duration;
        MagicHoleCast(manager); 
        magicHoleOnLastAttack = true;     
    }
}
