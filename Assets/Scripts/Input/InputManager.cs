using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    public event Action OnJumpAction;
    public event Action OnShootAction;
    public event Action OnReloadAction;
    public event Action OnFocusActionStarted;
    public event Action OnFocusActionEnded;
    public event Action OnSprintActionStarted;
    public event Action OnSprintActionEnded;
    public event Action OnHeadlightAction;
    public event Action OnInteract;

    public event Action OnPauseAction;


    PlayerInputActions playerInputActions;
    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();      
    }


    public float GetMovementDirection()
    {
         return playerInputActions.Player.Movement.ReadValue<float>();
    }

    public Vector2 GetQTEActions()
    {
        if(playerInputActions.Player.QTE.WasPressedThisFrame())
            return playerInputActions.Player.QTE.ReadValue<Vector2>();

        return Vector2.zero;
    }

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerInputActions.Player.Focus.started += Focus_started;
        playerInputActions.Player.Focus.canceled += Focus_canceled;
        playerInputActions.Player.Sprint.started += Sprint_started;
        playerInputActions.Player.Sprint.canceled += Sprint_canceled;
        playerInputActions.Player.Headlight.performed += Headlight_performed;
        playerInputActions.Player.Pause.performed += Pause_performed;
        playerInputActions.Player.Interact.performed += Interact_performed;

        playerHealth = GameManager.Instance.GetPlayerReference().GetComponentInChildren<PlayerHealth>();
        playerHealth.OnDeath += InputManager_OnPlayerDied;

        PlayerMovement.OnInCutscene += PlayerMovement_OnInCutscene;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Focus.started -= Focus_started;
        playerInputActions.Player.Focus.canceled -= Focus_canceled;
        playerInputActions.Player.Sprint.started -= Sprint_started;
        playerInputActions.Player.Sprint.canceled -= Sprint_canceled;
        playerInputActions.Player.Headlight.performed -= Headlight_performed;
        playerInputActions.Player.Pause.performed -= Pause_performed;
        playerInputActions.Player.Interact.performed -= Interact_performed;

        playerHealth.OnDeath -= InputManager_OnPlayerDied;
        PlayerMovement.OnInCutscene -= PlayerMovement_OnInCutscene;
    }

    private void PlayerMovement_OnInCutscene()
    {
        playerInputActions.Player.Disable();
    }

    private void InputManager_OnPlayerDied()
    {
        playerInputActions.Player.Disable();
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke();
    }

    private void Headlight_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnHeadlightAction?.Invoke();
    }

    private void Sprint_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintActionStarted?.Invoke();
    }

    private void Sprint_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnSprintActionEnded?.Invoke();
    }
    private void Update()
    {
        if(playerInputActions.Player.Jump.triggered)
        {
            OnJumpAction?.Invoke();
        }
        if(playerInputActions.Player.Shoot.triggered)
        {
            OnShootAction?.Invoke();
        }
        if(playerInputActions.Player.Reload.triggered)
        {
            OnReloadAction?.Invoke();
        }
        if (playerInputActions.Player.Focus.IsPressed())
        {
            Idle.ReportAction();
        }
    }

    private void Focus_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFocusActionStarted?.Invoke();
    }

    private void Focus_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnFocusActionEnded?.Invoke();
    }
}
