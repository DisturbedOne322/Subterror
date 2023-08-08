using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerVisuals : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Animator animator;

    private const string ATTACK_TRIGGER = "OnAttack";

    private float defaultAlpha;

    private float maxAlpha = 1;

    private float step = 0.05f;
    private float stepTime = 0.05f;

    private float lastLightenTime;
    private float dissapearAfterTime = 2.5f;

    private bool underLight = false;

    public event Action<bool> OnLighten;

    private ExecutionerApproachPlayer approachPlayer;

    private FocusedHeadlight focusedHeadlight;

    private void Start()
    {
        focusedHeadlight = GameManager.Instance.GetPlayerReference().GetComponentInChildren<FocusedHeadlight>();
        focusedHeadlight.OnExecutionerFound += FocusedHeadlight_OnExecutionerFound;
        approachPlayer = GetComponent<ExecutionerApproachPlayer>();
        approachPlayer.OnPlayerInRange += ApproachPlayer_OnPlayerInRange;
        animator = GetComponent<Animator>();
    }

    private void OnDestroy()
    {
        if (focusedHeadlight != null)
        {
            focusedHeadlight.OnExecutionerFound -= FocusedHeadlight_OnExecutionerFound;
            approachPlayer.OnPlayerInRange -= ApproachPlayer_OnPlayerInRange;
        }
    }

    private void ApproachPlayer_OnPlayerInRange(bool inRange)
    {
        if(inRange)
        {
            animator.SetTrigger(ATTACK_TRIGGER);
        }
    }

    private void FocusedHeadlight_OnExecutionerFound(ExecutionerVisuals obj)
    {
        if (obj != this)
            return;

        lastLightenTime = Time.time;

        if (underLight)
            return;

        underLight = true;
        StartCoroutine(IncreaseAlpha());
        OnLighten?.Invoke(true);
    }

    private IEnumerator IncreaseAlpha()
    {
        float currentAlpha = spriteRenderer.color.a;
        for (; currentAlpha < maxAlpha; currentAlpha += step)
        {
            Color temp = spriteRenderer.color;
            temp.a = currentAlpha;

            spriteRenderer.color = temp;

            yield return new WaitForSeconds(stepTime);
        }
    }

    private IEnumerator ReduceAlpha()
    {
        float currentAlpha = spriteRenderer.color.a;
        for (; currentAlpha > defaultAlpha; currentAlpha -= step)
        {
            Color temp = spriteRenderer.color;
            temp.a = currentAlpha;

            spriteRenderer.color = temp;

            yield return new WaitForSeconds(stepTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lastLightenTime + dissapearAfterTime < Time.time)
        {
            if (!underLight)
                return;
            OnLighten?.Invoke(false);
            underLight = false;
            StartCoroutine(ReduceAlpha());
        }
    }

    private void OnEnable()
    {
        defaultAlpha = 0.25f;
        Color temp = spriteRenderer.color;
        temp.a = defaultAlpha;
        spriteRenderer.color = temp;
    }
}
