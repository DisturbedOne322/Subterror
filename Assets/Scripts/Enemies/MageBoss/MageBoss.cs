using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MageBoss : MonoBehaviour
{
    public event Action<int, int> OnHPChanged;
    public static event Action OnFightFinished;
    public event Action OnStageChanged;
    [SerializeField]
    public WeakPoint[] collidersArray;

    public const string APPEAR_ANIM = "Base Layer.Appear";
    public const string DAMAGED_ANIM = "OnDamaged";
    public const string LASER_END_TRIGGER = "LaserAttackEnd";
    public const string FLAMEBALL_ANIM = "Base Layer.FlameballCast";
    public const string LASER_ANIM = "Base Layer.LaserCast";
    public const string LASER_PREPARE_ANIM = "Base Layer.LaserCastPrepare";
    public const string LASER_END_ANIM = "Base Layer.LaserCastEnd";
    public const string EXCALIBUR_ATTACK_TRIGGER = "ExcaliburAttack";
    public const string MAGIC_HOLE_ATTACK_TRIGGER = "MagicHoleAttack";
    public const string FINISHED_FIGHT_TRIGGER = "OnFinishedFight";
    public const string DEFEAT_ANIM_BOOL = "OnDefeat";
    public Animator animator;

    [SerializeField]
    private GameObject laserArm;
    [SerializeField]    
    private GameObject laserLight;
    [SerializeField]
    private GameObject wandArm;
    [SerializeField]
    private GameObject wandLight;

    [SerializeField]
    public GameObject sword;
    [SerializeField] 
    private GameObject swordArm;

    [SerializeField]
    private GameObject magicHoleArm;
    [SerializeField]
    private GameObject magicHoleVFX;

    [SerializeField]
    public Excalibur excaliburAttack;
    [SerializeField]
    public Transform swordSpawnPoint;

    [SerializeField]
    public MagicHole magicHole;

    public FlameballspawnManager flameballspawnManager;
    [SerializeField]
    public Laser laser;

    [SerializeField]
    private Material material;

    private const string outlineColor = "_OutlineColor";
    private const string outlineThickness = "_OutlineThickness";

    private Vector4 normalColor = new Vector4(0,0,0,1);
    private Vector4 damagedColor = new Vector4(1,0,0,1);

    private readonly float normalThickness = 0;
    private readonly float damagedThickness = 1;

    public PlayerMovement player;

    private MageBossBaseState currentState;

    public MageBossFirstStageState firstStageState = new MageBossFirstStageState();
    public MageBossSecondStageState secondStageState = new MageBossSecondStageState();
    public MageBossThirdStageState thirdStageState = new MageBossThirdStageState();

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip flyUpAudioClip;
    [SerializeField]
    private AudioClip defeatAudioClip;
    [SerializeField]
    private AudioClip[] damagedAudioClipArray;
    [SerializeField]
    private AudioClip swordThrowSound;
    [SerializeField]
    private AudioClip swordPierceBossSound;
    [SerializeField]
    private AudioClip getPulledByTeleporterSound;
    [SerializeField]
    private AudioClip goDownSound;
    [SerializeField]
    private AudioClip appearAudioClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        flameballspawnManager = GetComponent<FlameballspawnManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        currentState = firstStageState;
        animator = GetComponent<Animator>();

        for(int i = 0; i < collidersArray.Length; i++)
        {
            collidersArray[i].OnWeakPointBroken += MageBoss_OnWeakPointBroken;
        }
        SetNormalOutline();
        currentState.EnterState(this, "None");
        audioSource.PlayOneShot(appearAudioClip);
        currentState.OnCoreDestroyed += CurrentState_OnCoreDestroyed;
        currentState.OnFightFinished += CurrentState_OnFightFinished;
    }

    private void OnDestroy()
    {
        currentState.OnCoreDestroyed -= CurrentState_OnCoreDestroyed;
        currentState.OnFightFinished -= CurrentState_OnFightFinished;
    }

    private void CurrentState_OnFightFinished()
    {
        OnFightFinished?.Invoke();
        audioSource.PlayOneShot(goDownSound);
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    //called from anim event
    private void SwordThrown()
    {
        sword.SetActive(false);
    }
    // called from stage 3
    public void SwordReturned()
    {
        sword.SetActive(true);
    }

    private void CurrentState_OnCoreDestroyed(int arg1, int arg2)
    {
        OnHPChanged?.Invoke(arg1,arg2);
    }

    public void PlayDefeatAudioClip()
    {
        audioSource.PlayOneShot(defeatAudioClip);
    }

    private void MageBoss_OnWeakPointBroken()
    {
        AnimatorClipInfo[] m_CurrentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        if(m_CurrentClipInfo.Length != 0)
        {
            if (m_CurrentClipInfo[0].clip.name == "Idle")
                animator.SetTrigger(DAMAGED_ANIM);
        }

        audioSource.PlayOneShot(damagedAudioClipArray[UnityEngine.Random.Range(0, damagedAudioClipArray.Length)]);
        material.SetColor(outlineColor, damagedColor);
        material.SetFloat(outlineThickness, damagedThickness);
        SetNormalOutline();
    }

    public void SwitchState(MageBossBaseState nextState)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(defeatAudioClip);
        OnStageChanged?.Invoke();
        currentState.OnCoreDestroyed -= CurrentState_OnCoreDestroyed;
        currentState.OnFightFinished -= CurrentState_OnFightFinished;
        string lastAttack = currentState.LastAttack;
        currentState = nextState;
        currentState.EnterState(this, lastAttack);
        currentState.OnCoreDestroyed += CurrentState_OnCoreDestroyed;
        currentState.OnFightFinished += CurrentState_OnFightFinished;
    }

    public void EnableSecondStageArms()
    {
        audioSource.Play();
        audioSource.PlayOneShot(appearAudioClip);
        laserArm.SetActive(true);
        laserLight.SetActive(true);
        wandArm.SetActive(true);
        wandLight.SetActive(true);
    }

    public void EnableThirdStageArms()
    {
        audioSource.Play();
        audioSource.PlayOneShot(appearAudioClip);
        sword.SetActive(true);
        swordArm.SetActive(true);
        magicHoleArm.SetActive(true);
        magicHoleVFX.SetActive(true);
    }

    private WaitForSeconds outlineReturnDelay = new WaitForSeconds(0.05f);

    IEnumerator ReturnToNormalOutline()
    {
        Color c = new Color();
        c.a = 1;
        for (float red = damagedThickness; red >= normalThickness; red -= 0.1f)
        {
            c.r = red;
            material.SetColor(outlineColor, c);
            yield return outlineReturnDelay;
        }
    }

    public void ResetColliders()
    {
        foreach(var collider in collidersArray)
        {
            collider.ResetWeakPoint();
        }
    }

    public void PlayFlyUpSound()
    {
        audioSource.PlayOneShot(flyUpAudioClip);
    }

    public void PlayPulledByTeleporterSound()
    {
        audioSource.PlayOneShot(getPulledByTeleporterSound);
    }

    public void PlaySwordPirceBossSound()
    {
        audioSource.PlayOneShot(swordPierceBossSound);
    }

    public void PlayThrowSwordSoundInstantly()
    {
        audioSource.PlayOneShot(swordThrowSound);
    }

    public void PlayThrowSwordSound()
    {
        StartCoroutine(PlaySoundWithDelay(swordThrowSound, 2f));
    }

    IEnumerator PlaySoundWithDelay(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);

        audioSource.PlayOneShot(clip);
    }

    private void SetNormalOutline()
    {
        StartCoroutine("ReturnToNormalOutline");
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
}
