using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flameball : MonoBehaviour
{
    private Transform spawnPos;
    private Vector2 fallDirection = new Vector2 (-0.77f, -0.77f);
    private float speed = 10;
    public float Speed
    {
        get;
        set;
    }
    public Animator animator;
    [SerializeField]
    public SpriteRenderer spriteRenderer;

    public PlayerMovement player;
    public PlayerHealth playerHealth;


    public AudioSource audioSource;
    private float maxDistanceVolume = 8;

    //rocks
    [SerializeField]
    public AudioClip[] fallSoundArray;
    [SerializeField]
    public AudioClip fallingSound;


    [SerializeField]
    private Sprite sprite;

    public FlameballBaseState currentState;
    public FlameballFallingState fallingState = new FlameballFallingState();
    public FlameballPuddleState puddleState = new FlameballPuddleState();

    private float spriteSizeMultiplier = 6;
    private float colliderOffset = -0.4f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();

        currentState = fallingState;
        currentState.EnterState(this);
    }
    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>(); 
    }

    private void Update()
    {
        currentState.UpdateState(this);
        audioSource.volume = DynamicSoundVolume.GetDynamicVolume(maxDistanceVolume, Mathf.Abs(transform.position.y - player.transform.position.y));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentState.OnTriggerStay2D(this, collision);
    }

    public void UpdateCollider()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(sprite.bounds.size.x * spriteSizeMultiplier / 1.5f, sprite.bounds.size.y * spriteSizeMultiplier / 2.5f);
        GetComponent<BoxCollider2D>().offset = new Vector2(0, colliderOffset);
    }

    public void SwitchState(FlameballBaseState nextState)
    {
        currentState = nextState;
        currentState.EnterState(this);
    }

    public void FallDown()
    {
        transform.Translate(-transform.up * speed * Time.deltaTime);
    }
}
