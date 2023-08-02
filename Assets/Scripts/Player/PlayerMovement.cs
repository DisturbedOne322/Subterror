using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event Action<StaminaState> OnStaminaStateChange;
    public event Action OnPlayerTeleported;
    public event Action OnPlayerTeleportedArrived;
    private Rigidbody2D rb2D;

    [SerializeField]
    private LayerMask groundLayerMask;

    public event Action<bool> OnEnterExitSafeZone;

    [SerializeField]
    private readonly float originalSpeed = 0.85f;
    private float moveSpeed = 0.85f;
    private readonly float sprintSpeed = 1.35f;
    private readonly float backwardMoveSpeedMultiplier = 0.89f;
    private float slopeSpeedBonus;

    private bool onSlope = false;

    private float stamina = 1;
    public float Stamina
    {
        get { return stamina; }
    }

    #region Stamina
    public enum StaminaState
    {
        Idle,
        Regen,
        Deplete
    }
    private readonly float jumpStaminaConsumption = 0.1f;
    private readonly float staminaRegen = 0.1f;
    private readonly float staminaSpentPerSecond = 0.1f;
    #endregion

    private bool isSprinting = false;
    private bool isMoving = false;
    private bool canMove = true;

    private readonly float jumpForce = 0.007f;
    private readonly float standingJumpForce = 0.009f;

    private CapsuleCollider2D capsuleCollider;

    private float distanceToTheGround;
    private bool isFalling = false;

    [SerializeField]
    private GameObject[] lightSources;

    public bool IsFalling
    {
        get { return isFalling; }
    }

    private bool isGrounded = true;
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    float movementDirection;
    private bool inAnimation = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        distanceToTheGround = capsuleCollider.bounds.extents.y / 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.OnJumpAction += InputManager_OnJumpAction;
        InputManager.Instance.OnSprintActionStarted += Instance_OnSprintActionStarted;
        InputManager.Instance.OnSprintActionEnded += Instance_OnSprintActionEnded;

        QTE.instance.OnQTEStart += QTE_OnQTEStart;
        QTE.instance.OnQTEEnd += QTE_OnQTEEnd;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnJumpAction -= InputManager_OnJumpAction;
        InputManager.Instance.OnSprintActionStarted -= Instance_OnSprintActionStarted;
        InputManager.Instance.OnSprintActionEnded -= Instance_OnSprintActionEnded;

        QTE.instance.OnQTEStart -= QTE_OnQTEStart;
        QTE.instance.OnQTEEnd -= QTE_OnQTEEnd;
    }

    private void QTE_OnQTEEnd(IQTECaller caller)
    {
        canMove = true;
    }

    private void QTE_OnQTEStart()
    {
        canMove = false;
    }

    private void Instance_OnSprintActionEnded()
    {
        isSprinting = false;
        moveSpeed = originalSpeed;
    }

    private void Instance_OnSprintActionStarted()
    {
        if(stamina > 0.1f)
        {
            isSprinting = true;
            moveSpeed *= sprintSpeed;
        }
    }

    private void InputManager_OnJumpAction()
    {
        if (inAnimation)
            return;
        if(!canMove)        
            return;
        

        if(IsGrounded)
        {
            if (stamina > jumpStaminaConsumption)
            {
                rb2D.AddForce(new Vector2(movementDirection, 1.5f) * (isMoving ? jumpForce : standingJumpForce), ForceMode2D.Impulse);
                stamina -= jumpStaminaConsumption;
            }
        }
    }

    private void TurnLightsOff()
    {
        for(int i = 0; i < lightSources.Length; i++)
        {
            lightSources[i].SetActive(false);
        }
    }

    private void TurnLightsOn()
    {
        for (int i = 0; i < lightSources.Length; i++)
        {
            lightSources[i].SetActive(true);
        }
    }


    public void GetTeleported()
    {
        canMove = false;
        rb2D.velocity = Vector2.zero;
        TurnLightsOff();
        OnPlayerTeleported?.Invoke();
    }

    private void OnTeleportArrived()
    {
        canMove = true;
        TurnLightsOn();
        OnPlayerTeleportedArrived?.Invoke();
    }


    // Update is called once per frame
    void Update()
    {
        if (!canMove)       
            return;

        movementDirection = InputManager.Instance.GetMovementDirection();

        isMoving = movementDirection != 0;

        if (isMoving)
        {
            float moveSpeedMultiplier = (movementDirection > 0 && transform.localScale.x > 0)
                || (movementDirection < 0 && transform.localScale.x < 0) ? 1 : backwardMoveSpeedMultiplier;

            if(onSlope)
                moveSpeedMultiplier += slopeSpeedBonus;

            if (!isGrounded)
                moveSpeedMultiplier /= Mathf.Max(0.75f, airTime * 10);

            rb2D.AddForce(new Vector2(movementDirection * moveSpeed * moveSpeedMultiplier * Time.deltaTime, 0));
            Idle.ReportAction();
        }
        isFalling = rb2D.velocity.y < -2f;
        isGrounded = CheckIsGrounded();

        if (isSprinting)
            DepleteStamina();
        if(IsGrounded)
            RegenStamina();
    }

    private void DepleteStamina()
    {
        if(isMoving)
        {
            stamina -= staminaSpentPerSecond * Time.deltaTime;
            OnStaminaStateChange?.Invoke(StaminaState.Deplete);
            if (stamina < 0)
            {
                stamina = 0;
                isSprinting = false;
                moveSpeed = originalSpeed;
            }
        }
    }

    private void RegenStamina()
    {
        if(!isMoving || !isSprinting)
        {
            OnStaminaStateChange?.Invoke(StaminaState.Regen);
            stamina += staminaRegen * Time.deltaTime;
            if (stamina > 1)
                stamina = 1;
        }
    }

    private bool CheckIsGrounded()
    {

        RaycastHit2D rayHit = Physics2D.BoxCast(capsuleCollider.bounds.center, new Vector2(1,2),0, Vector2.down, distanceToTheGround + 0.1f, groundLayerMask);
        if (rayHit)
        {
            float angle = Mathf.Abs(rayHit.collider.gameObject.transform.rotation.eulerAngles.z);
            onSlope = angle != 0;
            slopeSpeedBonus = Mathf.Abs(angle > 180 ? angle - 360: angle) / 25;
            return true;
        }
        else
        {
            StartCoroutine(CalculateAirTime());
            return false;
        }
    }

    private bool inAir = false;
    private float airTime = 0;

    private IEnumerator CalculateAirTime()
    {
        if (!inAir)
        {
            inAir = true;

            while (!isGrounded)
            {
                airTime += Time.deltaTime;
                yield return null;
            }

            inAir = false;
            airTime = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            TurnLightsOff();
            OnEnterExitSafeZone?.Invoke(true);
        }
        if (collision.gameObject.CompareTag("TeleportDestination"))
        {
            OnTeleportArrived();
        }
        if(collision.gameObject.CompareTag("Teleport"))
        {
            GetTeleported();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            TurnLightsOn();
            OnEnterExitSafeZone?.Invoke(false);
        }
    }
}
