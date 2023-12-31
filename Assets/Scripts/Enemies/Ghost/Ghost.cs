using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Ghost : Enemy, IQTECaller, IReactToLight
{
    public event Action OnAttack;
    public event Action OnGhostSpotted;
    public event Action OnBurstAttack;
    public static event Action<Transform> OnDangerTrackGhost;
    public event Action OnPlayerDetected;
    public static event Action OnStopDangerTrack;

    private BoxCollider2D boxCollider2D;
    private Rigidbody2D rb2D;

    //isAttacked is true when player lights the ghost
    private bool isDetected;
    //vulnerable when lighten up
    public bool Vulnerable
    {
        get { return isDetected; }
    }
    ParticleSystem attackParticle;

    private PlayerMovement player;

    private float burstAttackDurationTotal = 4;
    private float burstAttackDuration = 4;
    //when attacked with light, move to Y, then at high speed burst to player pos.x with -1 to 1 range
    private float moveToYCoord;
    private readonly float burstSpeed = 13f;

    private readonly float spawnOffset = 2f;

    private bool isBurstAttacking = false;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        attackParticle = GetComponent<ParticleSystem>();
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        healthPoints = 10;
        moveSpeed = 7f;

        player = GameManager.Instance.GetPlayerReference();
    }

    protected override void MoveToPlayer()
    {
    }
    private void Update()
    {
        if (isDetected)
        {
            if (transform.position.y < moveToYCoord && !isBurstAttacking)
            {
                transform.Translate(new Vector2(0, moveSpeed) * Time.deltaTime);
            }
            else
            {
                isDetected = false;
                isBurstAttacking = true;
                BurstAttack();
            }
        }
    }


    private void BurstAttack()
    {
        StartCoroutine(BurstAttackDuration());
        //reposition to random x pos from player in range
        Vector2 startPos = new Vector2();

        startPos.x = UnityEngine.Random.Range(0,1f) > 0.5f ? 
            GameManager.Instance.BottomLeftScreenBoundaries.x - spawnOffset: 
            GameManager.Instance.TopRightScreenBoundaries.x + spawnOffset;

        startPos.y = UnityEngine.Random.Range(0, 1f) > 0.5f ? 
            GameManager.Instance.BottomLeftScreenBoundaries.y - spawnOffset:
            GameManager.Instance.TopRightScreenBoundaries.y + spawnOffset;

        transform.position = startPos;

        OnBurstAttack?.Invoke();
        OnDangerTrackGhost?.Invoke(this.transform);

        Vector2 burstDirectionVector = player.transform.position - transform.position;
        burstDirectionVector.Normalize();

        float angle = Vector3.SignedAngle(transform.up, burstDirectionVector, Vector3.forward);
        transform.Rotate(0,0,angle);
        rb2D.AddForce(burstDirectionVector * burstSpeed, ForceMode2D.Impulse);
    }

    private IEnumerator BurstAttackDuration()
    {
        yield return new WaitForSeconds(3);
        OnStopDangerTrack?.Invoke();
        isBurstAttacking = false;
        gameObject.SetActive(false);
    }

    protected override void AttackPlayer()
    {
    }
    protected override void DetectPlayer()
    {
    }

    public override void GetDamage(int damage)
    {
        healthPoints -= damage;
        if(healthPoints < 0)
        {
            Hide();
        }
    }
    public void ReactToLight()
    {
        if (isDetected || isBurstAttacking)
            return;
        moveToYCoord = transform.position.y + 10;
        OnGhostSpotted?.Invoke();
        isDetected = true;
        attackParticle.Play();
    }
     

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.rotation = Quaternion.identity;
            rb2D.velocity = Vector2.zero;
            boxCollider2D.enabled = false;

            attackParticle.Play();

            if(!isBurstAttacking)
            {
                OnPlayerDetected?.Invoke();
            }
            OnStopDangerTrack?.Invoke();
            OnAttack?.Invoke();
            QTE.instance.StartQTE(this, QTE.QTE_TYPE.Reaction);
        }
    }

    private void Hide()
    {
        OnStopDangerTrack?.Invoke();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        boxCollider2D.enabled = true;
        isDetected = false;
        isBurstAttacking = false;
        burstAttackDuration = burstAttackDurationTotal;
    }
}
