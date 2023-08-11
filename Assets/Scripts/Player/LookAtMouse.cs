using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    private Quaternion originalRotation;
    private Quaternion lastRotation;
    private Vector3 lastMousePosition;

    [SerializeField]
    private float topLookAtAngleBound = 70;
    [SerializeField]
    private float bottomLookAtAngleBound = -50f;

    private readonly float returnToOriginalRotationSpeed = 2f;

    [SerializeField]
    private bool focusFeature;
    private bool focused;

    private bool playerDead = false;
    private bool inCutscene = false;

    public bool Focused
    {
        get { return focused; }
        private set { focused = value; }       
    }

    private void Awake()
    {
        originalRotation = transform.rotation;
    }

    private PlayerMovement player;
    private PlayerHealth playerHealth;

    private void Start()
    {
        if (focusFeature)
        {
            InputManager.Instance.OnFocusActionStarted += InputManager_OnFocusAction;
            InputManager.Instance.OnFocusActionEnded += InputManager_OnFocusActionEnded;
        }
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.OnDeath += Player_OnPlayerDied;
        PlayerMovement.OnInCutscene += PlayerMovement_OnInCutscene;
    }

    private void PlayerMovement_OnInCutscene()
    {
        inCutscene = true;
    }

    private void Player_OnPlayerDied()
    {
        playerDead = true;
    }

    private void OnDestroy()
    {
        if (focusFeature)
        {
            InputManager.Instance.OnFocusActionStarted -= InputManager_OnFocusAction;
            InputManager.Instance.OnFocusActionEnded -= InputManager_OnFocusActionEnded;
        }
        playerHealth.OnDeath -= Player_OnPlayerDied;
        PlayerMovement.OnInCutscene -= PlayerMovement_OnInCutscene;
    }

    private void InputManager_OnFocusActionEnded()
    {
        focused = false;
    }

    private void InputManager_OnFocusAction()
    {
        focused = true;
    }

    private void LateUpdate()
    {
        if (inCutscene)
            return;

        if (playerDead)
            return;

        if (GameManager.Instance.gamePaused)
            return;


        if (Input.mousePosition != lastMousePosition)
        {
            Idle.ReportAction();
            lastRotation = transform.localRotation;
        }

        if (Idle.IsIdle)
        {
            ReturnToOriginalPosition();
        }
        else
        {
            LookAt();
        }
    }

    private void LookAt()
    {
        Vector2 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);

        float bottomMaxAngle = bottomLookAtAngleBound;
        float topMaxAngle = topLookAtAngleBound;

        if (direction.x < 0)
        {
            direction = -direction;
            bottomMaxAngle = -topLookAtAngleBound;
            topMaxAngle = -bottomLookAtAngleBound;
        }


        if (focusFeature && focused)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, bottomMaxAngle, topMaxAngle);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        if (!focusFeature)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, bottomMaxAngle, topMaxAngle);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }


        lastMousePosition = Input.mousePosition;
    }

    private void ReturnToOriginalPosition()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, returnToOriginalRotationSpeed * Time.deltaTime);
    }
}
