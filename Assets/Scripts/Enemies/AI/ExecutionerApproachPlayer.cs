using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class ExecutionerApproachPlayer : MonoBehaviour
{
    private PlayerMovement player;
    private Rigidbody2D rb;

    public event Action<bool> OnPlayerInRange;

    //approach player until this distance
    [SerializeField]
    private float targetDistanceInitial;
    private float targetDistance;
    [SerializeField]
    private float verticalDistance = 1;

    private float speedMultiplierPerDistance;

    [SerializeField]
    private float maxFollowDistance;

    [SerializeField]
    private float maxTargetOffset = 0;

    [SerializeField]
    private float initialSpeed;
    private float speed;

    [SerializeField]
    private float cdAfterAttackTotal = 3;
    private float cdAfterAttack;
    private float lastAttackTime;

    private bool inCD = false;

    private bool isAlive = true;

    private IDamagable damagable;

    // Start is called before the first frame update
    void Start()
    {
        speed = initialSpeed; 
        rb = GetComponent<Rigidbody2D>();   
        player = GameManager.Instance.GetPlayerReference();
        damagable = GetComponentInParent<IDamagable>();
        damagable.OnDeath += Damagable_OnDeath;

        targetDistance = UnityEngine.Random.Range(targetDistanceInitial - maxTargetOffset, targetDistanceInitial);
    }

    private void OnDestroy()
    {
        if (damagable != null)
            damagable.OnDeath -= Damagable_OnDeath;
    }

    public void SetSpeed(float speed)
    {
        this.initialSpeed = speed;
    }

    private void Damagable_OnDeath()
    {
        isAlive = false;
        speed = 0;
        cdAfterAttack = 999f;
        rb.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        isAlive = true;
        cdAfterAttack = cdAfterAttackTotal;
        speed = initialSpeed;
    }

    private void FixedUpdate()
    {
        if(inCD)
        {
            if(Time.time > lastAttackTime + cdAfterAttack)            
                inCD = false;            
            return;
        }

        if(!isAlive)
        {
            return;
        }

        Vector2 vectorToPlayer = (player.transform.position - transform.position).normalized;

        float currentHorizontalDistance = Mathf.Abs(player.transform.position.x - transform.position.x);
        float currentVerticalDistance = Mathf.Abs(player.transform.position.y - transform.position.y);
        bool playerInRange = currentHorizontalDistance < targetDistance && currentVerticalDistance < verticalDistance;

        speedMultiplierPerDistance = Mathf.Clamp(Vector2.Distance(player.transform.position, transform.position) / (targetDistance * 2),1,5);

        if (currentHorizontalDistance > maxFollowDistance)
            return;

        if (!playerInRange)
            rb.AddForce(vectorToPlayer * speed * speedMultiplierPerDistance);
        else
        {
            rb.velocity = Vector3.zero;
            lastAttackTime = Time.time;
            inCD = true;
        }

        OnPlayerInRange?.Invoke(playerInRange);
    }
}
