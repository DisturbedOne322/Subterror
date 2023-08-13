using System;
using UnityEngine;

public class HellHound : Enemy
{
    public event Action OnAggressiveStateChange;
    public event Action OnHellHoundAttack;
    public event Action OnSuccessfulHit;
    public event Action OnDamageTaken;

    [SerializeField]
    private PlayerInRange playerDetection;

    private Rigidbody2D rb;

    private float getAggressiveTimer ;

    private float walkSpeed = 0.7f;
    private float runSpeed;
    private float runSpeedMin = 5.75f;
    private float runSpeedMax = 8f;
    private float dyingSpeedMultiplier = 1f;
    private float attackCD = -1;
    private float attackCDTotal = 1.5f;

    private float attackDistance = 3f;
    private float distanceToPlayer;

    private float maxFollowRange = 30;

    private bool hasAttackedRecently = false;

    public bool isAlive = true;

    private PlayerMovement player;
    private PlayerHealth playerHealth;
    [SerializeField]
    private Dissolve dissolve;

    private void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();
        moveSpeed = walkSpeed;
        healthPoints = 3;
        attackRange = 0.2f;
        damage = 1;
        getAggressiveTimer = UnityEngine.Random.Range(2f, 3.5f);
        runSpeed = UnityEngine.Random.Range(runSpeedMin, runSpeedMax);
        playerDetection.OnPlayerInRange += PlayerDetection_OnPlayerInRange;
        dissolve.OnDissolved += Dissolve_OnDissolved;
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), player.GetComponent<CapsuleCollider2D>());
    }

    private void OnDestroy()
    {
        playerDetection.OnPlayerInRange -= PlayerDetection_OnPlayerInRange;
        dissolve.OnDissolved -= Dissolve_OnDissolved;
    }

    private void Dissolve_OnDissolved()
    {
        Destroy(gameObject);
    }

    private void PlayerDetection_OnPlayerInRange(PlayerMovement player)
    {
        hasDetectedPlayer = true;
    }

    private void Update()
    {
        if (hasDetectedPlayer)
        {
            distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);
            if(distanceToPlayer > 0.5f)
                MoveToPlayer();

            getAggressiveTimer -= Time.deltaTime;

            if (getAggressiveTimer <= 0)
            {
                OnAggressiveStateChange?.Invoke();

                attackCD -= Time.deltaTime;
                if (attackCD < 0)
                {
                    hasAttackedRecently = false;
                    moveSpeed = runSpeed;
                }

                if (distanceToPlayer < attackDistance)
                {
                    AttackPlayer();
                }
            }

            dyingSpeedMultiplier = dissolve.DissolveOnDeath();
        }
    }
    protected override void DetectPlayer()
    {
    }
    protected override void MoveToPlayer()
    {
        if (Vector2.Distance(transform.position, player.transform.position) > maxFollowRange)
            return;
        transform.Translate(new Vector2((transform.position.x > player.transform.position.x ? -1 : 1) * moveSpeed * dyingSpeedMultiplier * Time.deltaTime,0));
    }
    protected override void AttackPlayer()
    {
        if (!isAlive)
            return;

        if(attackCD < 0 && !hasAttackedRecently)
        {
            rb.AddForce(new Vector2(-transform.localScale.x, 0) * 20, ForceMode2D.Impulse);
            OnHellHoundAttack?.Invoke();
            attackCD = attackCDTotal;
            hasAttackedRecently = true;
            moveSpeed = walkSpeed;
        }
    }
    public override void GetDamage(int damage)
    {
        if (healthPoints <= 0)
            return;

        healthPoints -= damage;
        if (healthPoints <= 0)
        {
            dissolve.StartDissolving();
            isAlive = false;
        }

        OnDamageTaken?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && hasAttackedRecently)
        {
            hasAttackedRecently = false;
            OnSuccessfulHit?.Invoke();
            playerHealth.TakeDamage(damage);
        }
    }
}
