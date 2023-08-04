using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Excalibur : MonoBehaviour
{
    public event Action OnSwordAttackFinished;


    private const string GROUND_HIT_ANIM_TRIGGER = "GhoundHit";
    private const string DURATION_ENDED_TRIGGER = "DurationEnded";

    private const string CRACK_APPEAR_ANIM_TRIGGER = "OnAppear";
    private const string CRACK_DISAPPEAR_ANIM_TRIGGER = "OnDisappear";

    private Animator animator;
    private Rigidbody2D rb;

    private PlayerMovement player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRB;

    private float fallSpeed = 1000;

    private bool hitTheGround = false;

    private readonly Vector3 circleCastOffset = new Vector3(-1, -2, 0);
    private readonly float circleCastRadius = 4;

    private float stayOnGroundDuration;

    private bool posAligned = false;
    private bool angleAligned = false;

    private TrailRenderer trailRenderer;

    //return to boss' hands
    [SerializeField]
    private GameObject target;

    private Vector2 positionVelocitySmDamp;
    private float angleVelocitySmDamp;

    private Vector3 defaultScale;
    private Vector3 sizeSmDampVelocity;
    private float sizeSmDampTime = 1f;

    [SerializeField]
    private GameObject floorCrack;

    private bool returning = false;

    //audio source plays fire sound
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip hitGroundSound;
    [SerializeField]
    private AudioClip fallingDownSound;
    [SerializeField]
    private AudioClip returnSound;

    private readonly float throwBackForce = 0.001f;
    private readonly float explosionForce = 0.015f;

    //max distance when explosion force has any impact
    private readonly float maxExplosionForceDistance = 15f;


    [SerializeField]
    private Material dissolveSwordMat;

    private const string DISSOLVE_AMOUNT = "_dissolveAmount";

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();       
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>(); 
        playerRB = player.GetComponent<Rigidbody2D>();
        MageBoss.OnFightFinished += MageBoss_OnFightFinished;
    }

    private void OnDestroy()
    {
        MageBoss.OnFightFinished -= MageBoss_OnFightFinished;
    }

    private void MageBoss_OnFightFinished()
    {
        if(!returning)
        {
            stayOnGroundDuration = 999;
            animator.SetTrigger(DURATION_ENDED_TRIGGER);
            StartCoroutine(Dissolve());
        }
        else
        {
            returning = false;
            StartCoroutine(Dissolve());
        }
    }

    private IEnumerator Dissolve()
    {
        for(float i = 1; i > 0; i-= 0.01f) 
        {
            dissolveSwordMat.SetFloat(DISSOLVE_AMOUNT, i);
            yield return new WaitForSeconds(0.02f);
        }
        gameObject.SetActive(false);
    }

    public void Initialize(Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        posAligned = false;
        angleAligned = false;
        hitTheGround = false;
        returning = false;
        fallSpeed = 500f;
        rb.freezeRotation = false;
        stayOnGroundDuration = 8f;
        player = GameManager.Instance.GetPlayerReference();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
        trailRenderer.enabled = true;

        transform.right = -(player.transform.position - transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitTheGround)
        {
            rb.AddForce(-transform.right * fallSpeed * Time.deltaTime);
        }
        else
        {
            if(!returning)
            {
                stayOnGroundDuration -= Time.deltaTime;
                if (stayOnGroundDuration <= 0)
                {
                    floorCrack.GetComponent<Animator>().SetTrigger(CRACK_DISAPPEAR_ANIM_TRIGGER);
                    animator.SetTrigger(DURATION_ENDED_TRIGGER);
                    audioSource.Stop();
                    audioSource.PlayOneShot(returnSound);
                    returning = true;
                }
            }
        }

        if(returning)
        {
            ReturnToDefaultSize();
            AlignPosition();
            AlignRotation();
            if (posAligned && angleAligned)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        audioSource.PlayOneShot(fallingDownSound);
        defaultScale = transform.localScale;
        transform.localScale *= 2;
    }

    private void ReturnToDefaultSize()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, defaultScale, ref sizeSmDampVelocity, sizeSmDampTime);
    }

    private void AlignPosition()
    {
        if (!posAligned)
        {
            transform.position = Vector2.SmoothDamp(transform.position, target.transform.position, ref positionVelocitySmDamp, 1f);

            if(transform.position.x <= target.transform.position.x + 0.1f && transform.position.x >= target.transform.position.x -0.1f)
            {
                if(transform.position.y <= target.transform.position.y + 0.1f && transform.position.y >= target.transform.position.y -0.1f)
                {
                   posAligned = true;
                }
            }
        }
    }

    private void AlignRotation()
    {
        if (!angleAligned)
        {
            float currentAngle = transform.rotation.eulerAngles.z;
            float targetAngle = target.transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.SmoothDamp(currentAngle, targetAngle, ref angleVelocitySmDamp, 1f)));

            if(transform.eulerAngles.z <= target.transform.eulerAngles.z + 2 && transform.eulerAngles.z >= target.transform.eulerAngles.z - 2)
            {
                angleAligned = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector2 throwDirection = new Vector2(0, throwBackForce);
            float playerPosX = player.transform.position.x;
            throwDirection.x = playerPosX < transform.position.x ? -throwBackForce : throwBackForce;

            playerRB.AddForce(throwDirection, ForceMode2D.Impulse);
            playerHealth.TakeDamage(1);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnSwordAttackFinished?.Invoke();

            RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position + circleCastOffset, circleCastRadius, Vector2.up);
            
            foreach(var objectHit in hit)
            {
                if (objectHit.collider.gameObject.CompareTag("Player"))
                {
                    playerHealth.TakeDamage(2);
                    //since player has multiple colliders, he gets damaged multiple times, so break early from the loop
                    break;
                }
            }

            audioSource.PlayOneShot(hitGroundSound);
            audioSource.Play();
            animator.SetTrigger(GROUND_HIT_ANIM_TRIGGER);

            rb.velocity = Vector2.zero;
            fallSpeed = 0;

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if(distanceToPlayer == 0)
            {
                distanceToPlayer = 0.01f;
            }
            float explosionForceMultiplier = Mathf.Clamp01( 1 - (distanceToPlayer / maxExplosionForceDistance));

            Vector2 throwDirection1 = new Vector2(explosionForce * explosionForceMultiplier, explosionForce * explosionForceMultiplier);
            float playerPosX = player.transform.position.x;
            throwDirection1.x *= playerPosX < transform.position.x ? -1 : 1;
            playerRB.AddForce(throwDirection1, ForceMode2D.Impulse);

            floorCrack.gameObject.SetActive(true);
            floorCrack.transform.position = collision.GetContact(0).point;
            floorCrack.GetComponent<Animator>().SetTrigger(CRACK_APPEAR_ANIM_TRIGGER);
            trailRenderer.enabled = false;
            hitTheGround = true;
            
            rb.freezeRotation = true;
        }
    }
}
