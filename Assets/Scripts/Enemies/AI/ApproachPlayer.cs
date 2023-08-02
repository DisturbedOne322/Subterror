using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApproachPlayer : MonoBehaviour
{
    private PlayerMovement player;
    private Rigidbody2D rb;

    public event Action<bool> OnPlayerInRange;

    //approach player until this distance
    [SerializeField]
    private float targetDistanceInitial;
    private float targetDistance;

    private float speedMultiplierPerDistance;

    [SerializeField]
    private float maxFollowDistance;

    [SerializeField]
    private float maxTargetOffset = 0;

    [SerializeField]
    private float initialSpeed;
    private float speed;

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
        rb.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        isAlive = true;
        speed = initialSpeed;
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }

        Vector2 vectorToPlayer = (player.transform.position - transform.position).normalized;

        float currentHorizontalDistance = Mathf.Abs(player.transform.position.x - transform.position.x);
        float distance = Vector2.Distance(player.transform.position, transform.position);
        bool playerInRange = distance < targetDistance;

        speedMultiplierPerDistance = Mathf.Clamp(Vector2.Distance(player.transform.position, transform.position) / (targetDistance * 2), 1, 5);

        if (distance > maxFollowDistance)
            return;

        if (!playerInRange)
            rb.AddForce(vectorToPlayer * speed * speedMultiplierPerDistance);
        else
        {
            rb.velocity = Vector3.zero;
        }

        OnPlayerInRange?.Invoke(playerInRange);
    }
}
