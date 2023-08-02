using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRange : MonoBehaviour
{
    public event Action<PlayerMovement> OnPlayerInRange;
    [SerializeField]
    private Transform leftDetectionPoint;
    [SerializeField] 
    private Transform rightDetectionPoint;

    private bool foundPlayer = false;

    [SerializeField]
    private LayerMask playerLayerMask;

    HellHound hellHound;

    private void FixedUpdate()
    {
        FindPlayerInRange();
    }

    private void Start()
    {
        hellHound = gameObject.GetComponentInParent<HellHound>();
        hellHound.OnDamageTaken += HellHound_OnDamageTaken;
    }

    private void OnDestroy()
    {
        hellHound.OnDamageTaken -= HellHound_OnDamageTaken;
    }

    public void OnAnotherHellHoundDamaged()
    {
        if (foundPlayer)
            return;

        OnPlayerInRange?.Invoke(GameManager.Instance.GetPlayerReference());
        foundPlayer = true;
    }

    private void HellHound_OnDamageTaken()
    {
        if (!foundPlayer)
        {
            NotifyNearbyHound();
            OnPlayerInRange?.Invoke(GameManager.Instance.GetPlayerReference());
            foundPlayer = true;
        }
    }

    private void FindPlayerInRange()
    { 
        Collider2D collider = Physics2D.OverlapArea(leftDetectionPoint.position, rightDetectionPoint.position, playerLayerMask);

        if (collider != null && !foundPlayer) 
        {
            NotifyNearbyHound();
            foundPlayer = true;
            OnPlayerInRange?.Invoke(collider.gameObject.GetComponent<PlayerMovement>());
        }
    }

    private void NotifyNearbyHound()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, new Vector2(10, 10), 0, Vector3.zero);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.TryGetComponent<HellHound>(out HellHound anotherHellHound))
            {
                anotherHellHound.GetComponentInChildren<PlayerInRange>().OnAnotherHellHoundDamaged();
            }
        }
    }
}
