using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public event Action OnAppear;
    public event Action OnDisappear;

    public static event Action OnTeleporterImpulse;

    private TeleporterSoundManager soundManager;

    private PlayerMovement player;

    private LineRenderer lineRenderer;

    [SerializeField]
    private Color lineColor;

    [SerializeField]
    private Transform pullingHandPosition;

    [SerializeField]
    private Material teleporterMaterial;
    private const string MATERIAL_ALPHA = "_Alpha";

    private float distanceToPlayer;
    private readonly float  distanceToPullPlayer = 8f;
    private readonly float distanceToTeleportPlayer = 1.1f;


    //pull player when he is close
    private bool isPullingPlayer= false;
    private readonly float initialPullingForce = 0.75f;
    private readonly float pullingForceIncreasePerSecond = 0.5f;
    private readonly float pullingForceChangeTimerTotal = 1f;
    private float pullingImpulseForceTimer = 0.5f;


    private float currentImpulseForce;

    // Start is called before the first frame update
    void Start()
    {
        soundManager = GetComponent<TeleporterSoundManager>();
        player = GameManager.Instance.GetPlayerReference();
        lineRenderer = GetComponent<LineRenderer>();
        currentImpulseForce = initialPullingForce;
        pullingImpulseForceTimer = pullingForceChangeTimerTotal;
        teleporterMaterial.SetFloat(MATERIAL_ALPHA, 1);

        Color temp = lineColor;
        temp.a = 0;
        lineRenderer.SetColors(temp, temp);

    }

    // Update is called once per frame
    void Update()
    {
        if (!isPullingPlayer)
        {
            distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
            if (distanceToPlayer < distanceToPullPlayer)
            {
                OnAppear?.Invoke();
            }
        }
        else
        {
            PullThePlayer();
            SetLineRendererPosition();
        }
    }

    private void StartPulling()
    {
        isPullingPlayer = true;
    }

    private IEnumerator DecreaseTextAlpha()
    {
        float i = 1;
        while(i > 0)
        {
            teleporterMaterial.SetFloat(MATERIAL_ALPHA, i);
            i -= Time.deltaTime * 3;
            yield return null;
        }
        teleporterMaterial.SetFloat(MATERIAL_ALPHA, 0);
    }


    private void PullThePlayer()
    {
        Vector3 pullDirection = transform.position - player.transform.position;

        pullingImpulseForceTimer -= Time.deltaTime;

        if(pullingImpulseForceTimer <= 0)
        {
            pullingImpulseForceTimer = pullingForceChangeTimerTotal;
            currentImpulseForce += pullingForceIncreasePerSecond;
            player.GetComponent<Rigidbody2D>().AddForce(pullDirection.normalized * currentImpulseForce * Time.deltaTime, ForceMode2D.Impulse);
            soundManager.PlayImpulseSound();
            OnTeleporterImpulse?.Invoke();
            StartCoroutine(DoImpulseAttack());
        }


        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        if (distanceToPlayer < distanceToTeleportPlayer)
        {
            StartCoroutine(DecreaseTextAlpha());
            player.GetTeleported();
            OnDisappear?.Invoke();
            isPullingPlayer = false;
            StopAllCoroutines();
            Color temp = lineColor;
            temp.a = 0;
            lineRenderer.SetColors(temp, temp);
        }
    }

    private IEnumerator DoImpulseAttack()
    {
        lineRenderer.SetColors(lineColor, lineColor);
        Color temp = lineColor;
        while(temp.a > 0)
        {
            temp.a -= Time.deltaTime;
            lineRenderer.SetColors(temp, temp);
            yield return null;
        }
        temp.a = 0;
        lineRenderer.SetColors(temp, temp);
    }

    private void SetLineRendererPosition()
    {
        lineRenderer.SetPosition(0, pullingHandPosition.position);
        lineRenderer.SetPosition(1, player.transform.position);
    }

    private void Disappear()
    {
        isPullingPlayer = false;
        gameObject.SetActive(false);
    }
}
