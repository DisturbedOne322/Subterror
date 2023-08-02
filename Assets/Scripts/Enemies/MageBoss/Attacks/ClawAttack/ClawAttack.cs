using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawAttack : MonoBehaviour
{
    [SerializeField]
    private PlayerInClawAttackRange playerInClawAttackRange;

    private float attackAnimationCD = 0;
    private float attackAnimationCDTotal = 1.5f;

    private float damageCD = 0.5f;
    private float damageCDTotal = 0.5f;

    public const string CLAW_ATTACK_TRIGGER = "ClawAttack";

    [SerializeField]
    private Transform attackRange;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private AudioClip attackSound;

    private AudioSource audioSource;

    private PlayerMovement player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRB;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRB = player.GetComponent<Rigidbody2D>();
        playerInClawAttackRange.OnPlayerInClawAttackRange += PlayerInClawAttackRange_OnPlayerInClawAttackRange;
    }
    private void OnDestroy()
    {
        playerInClawAttackRange.OnPlayerInClawAttackRange -= PlayerInClawAttackRange_OnPlayerInClawAttackRange;
    }

    private void PlayerInClawAttackRange_OnPlayerInClawAttackRange()
    {
        if (!gameObject.activeSelf)
            return;
        if (attackAnimationCD > 0)
            return;
        audioSource.PlayOneShot(attackSound);
        animator.SetTrigger(CLAW_ATTACK_TRIGGER);
        attackAnimationCD = attackAnimationCDTotal;
    }

    // Update is called once per frame
    void Update()
    {
        attackAnimationCD -= Time.deltaTime;
        damageCD -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageCD > 0)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.TakeDamage(2);
            playerRB.AddForce(new Vector2(-0.01f,0.01f), ForceMode2D.Impulse);
            damageCD = damageCDTotal;
        }
    }
}
