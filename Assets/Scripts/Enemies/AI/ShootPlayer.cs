using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPlayer : MonoBehaviour, IParticleSpawnerCaller, IShootableWeapon
{
    public event Action OnSpawnParticleAction;
    public event Action OnShoot;

    [SerializeField]
    private Transform parent;

    private PlayerMovement player;
    private PlayerHealth playerHealth;
    [SerializeField]
    private LayerMask layerMask;
    [SerializeField]
    private ApproachPlayer approachPlayer;

    private bool canShoot = true;
    private float shootCD = 2;
    private float shootDistance = 20f;

    private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();
        approachPlayer.OnPlayerInRange += ApproachPlayer_OnPlayerInRange;
    }

    private void OnDestroy()
    {
        approachPlayer.OnPlayerInRange -= ApproachPlayer_OnPlayerInRange;
    }

    private void ApproachPlayer_OnPlayerInRange(bool inRange)
    {
        if (!canShoot)
            return;

        if(inRange)
            TryShootPlayer();
    }

    private void TryShootPlayer()
    {
        if (player == null)
            return;

        StartCoroutine(ShootCooldown(shootCD));
        canShoot = false;

        OnSpawnParticleAction?.Invoke();
        OnShoot?.Invoke();

        SoundManager.Instance.PlayShootSound();

        hit = Physics2D.Raycast(transform.position, transform.right * parent.transform.localScale.x, shootDistance, layerMask);
        if(hit)
        {
                playerHealth.TakeDamage(1);
        }
    }

    private IEnumerator ShootCooldown(float cd)
    {
        yield return new WaitForSeconds(cd);
        canShoot = true;
    }
}
