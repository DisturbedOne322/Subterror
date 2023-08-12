using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleStateManager : MonoBehaviour, IQTECaller, IReactToLight
{
    public Animator animator;
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public PlayerMovement player;
    public PlayerHealth playerHealth;
    [SerializeField]
    private Transform playerHoldPoint;


    private TentaclesBaseState currentState;
    private TentaclesBaseState prevState;

    public TentacleAttackState attackState = new TentacleAttackState();
    public TentacleDisappearState disappearState = new TentacleDisappearState();
    public TentacleIdleState idleState = new TentacleIdleState();
    public TentacleShrinkState shrinkState = new TentacleShrinkState();
    public TentacleSpreadState spreadState = new TentacleSpreadState();

    public bool underLight = false;
    private float lastLightTime;
    private float shrinkDelay = 0.25f;

    public bool pullingPlayer = false;
    public bool playerFreed = false;

    [SerializeField]
    public LayerMask playerLayer;

    private AudioSource audioSource;


    [SerializeField]
    private AudioClip idleSound;
    [SerializeField]
    private AudioClip shrinkAndSpreadSound;

    public enum InitialState
    {
        attackState,
        idleState
    }

    [SerializeField]
    public InitialState initialState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        currentState = initialState == InitialState.attackState? attackState:idleState;
        prevState = currentState;
        currentState.EnterState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        //FocusedHeadlight.OnTentacleFound += FocusedHeadlight_OnTentacleFound;
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>(); 
        QTE.instance.OnQTEEnd += Instance_OnQTEEnd;
    }

    private void OnDestroy()
    {
        //FocusedHeadlight.OnTentacleFound -= FocusedHeadlight_OnTentacleFound;
        QTE.instance.OnQTEEnd -= Instance_OnQTEEnd;
    }

    public void ReactToLight()
    {
        underLight = true;
        lastLightTime = Time.time;
    }

    private void Instance_OnQTEEnd(IQTECaller caller)
    {
        if(caller == this)
        {
            pullingPlayer = false;
            playerFreed = true;
            SwitchState(disappearState);
        }
    }

    //private void FocusedHeadlight_OnTentacleFound(TentacleStateManager obj)
    //{
    //    if (obj == this)
    //    {
    //        underLight = true;
    //        lastLightTime = Time.time;
    //    }
    //}
    
    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        underLight = lastLightTime + shrinkDelay > Time.time;

        Vector2 spriteSize = spriteRenderer.bounds.size;
        spriteSize.y *= 2;

        boxCollider2D.size = spriteSize;
        Vector2 newColliderPosition = new Vector2(0, spriteRenderer.gameObject.transform.localPosition.y);
        boxCollider2D.offset = newColliderPosition;

        if (pullingPlayer && !playerFreed)
        {
            player.transform.position = playerHoldPoint.position;
        }
    }

    public void SwitchState(TentaclesBaseState newState)
    {
        prevState = currentState;
        currentState = newState;
        currentState.EnterState(this);
    }

    public TentaclesBaseState GetPrevState() 
    {
        return prevState;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }

    public void SetIdleSound()
    {
        audioSource.clip = idleSound;
    }

    public void SetShrinkSpreadSound()
    {
        audioSource.clip = shrinkAndSpreadSound;
    }
}
