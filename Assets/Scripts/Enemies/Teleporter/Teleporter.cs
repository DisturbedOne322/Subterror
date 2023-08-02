using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public event Action OnAppear;
    public event Action OnDisappear;

    private PlayerMovement player;

    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform pullingHandPosition;

    private float distanceToPlayer;
    private readonly float  distanceToPullPlayer = 6f;
    private readonly float distanceToTeleportPlayer = 0.8f;


    //pull player when he is close
    private bool isPullingPlayer= false;
    private readonly float initialPullingForce = 0.85f;
    private readonly float pullingForceIncreasePerSecond = 0.85f;
    private readonly float pullingForceChangeTimerTotal = 1f;
    private float pullingForceChangeTimer;


    private float currentPullingForce;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        lineRenderer = GetComponent<LineRenderer>();
        currentPullingForce = initialPullingForce;
        pullingForceChangeTimer = pullingForceChangeTimerTotal;
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
            DisplayLinkToPlayer();
        }
    }

    private void StartPulling()
    {
        isPullingPlayer = true;
        lineRenderer.enabled = true;
    }


    private void PullThePlayer()
    {
        Vector3 pullDirection = transform.position - player.transform.position;

        pullingForceChangeTimer -= Time.deltaTime;

        if(pullingForceChangeTimer <= 0)
        {
            pullingForceChangeTimer = pullingForceChangeTimerTotal;
            currentPullingForce += pullingForceIncreasePerSecond;
        }

        player.GetComponent<Rigidbody2D>().AddForce(pullDirection.normalized * currentPullingForce * Time.deltaTime);

        distanceToPlayer = Mathf.Abs(pullDirection.x);

        if (distanceToPlayer < distanceToTeleportPlayer)
        {
            player.GetTeleported();
            OnDisappear?.Invoke();
            isPullingPlayer = false;
            lineRenderer.enabled = false;
        }
    }

    private void DisplayLinkToPlayer()
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
