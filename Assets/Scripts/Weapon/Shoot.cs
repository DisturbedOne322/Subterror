using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public static event Action<GameObject,LayerMask, Vector3> OnBulletHit;

    public static event Action<int,int> OnSuccessfulShoot;
    public static event Action<int> OnSuccessfulReload;
    public static event Action OnWeaponJammed;

    private Transform playerPos;

    private PlayerMovement player;
    [SerializeField]
    private LayerMask shootableLayerMasks;

    private bool isPlayerInQTE;

    [SerializeField]
    private Transform shootStartingPoint;
    [SerializeField]
    private Transform shootTargerPoint;

    [SerializeField] 
    private ParticleSystem barrellParticles;

    [SerializeField]
    private ParticleSystem sparksParticles;

    private float timerToSpawnSmoke;
    private float timeToSpawnSmoketotal = 0.4f;
    private bool isShooting = false;
    private float shootDistance = 50f;

    private int magCount = 12;
    public int currentBulletNum = 12;

    private bool reloading = false;
    private float reloadTimer = 2f;
    private float reloadTimerTotal = 2f;

    private float gunJamProbability = 0.0075f;
    private bool isJammed = false;

    [SerializeField]
    private EnemySpawnManager spawnManager;

    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerPos = player.transform;
        InputManager.Instance.OnShootAction += InputManager_OnShootAction;
        InputManager.Instance.OnReloadAction += InputManager_OnReloadAction;
        spawnManager.OnMiniBossFightStarted += SpawnManager_OnMiniBossFightStarted;
        spawnManager.OnBossFightFinished += SpawnManager_OnBossFightFinished;

        QTE.instance.OnQTEStart += QTE_OnQTEStart;
        QTE.instance.OnQTEEnd += Instance_OnQTEEnd;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnShootAction -= InputManager_OnShootAction;
        InputManager.Instance.OnReloadAction -= InputManager_OnReloadAction;
        spawnManager.OnMiniBossFightStarted -= SpawnManager_OnMiniBossFightStarted;
        spawnManager.OnBossFightFinished -= SpawnManager_OnBossFightFinished;

        QTE.instance.OnQTEStart -= QTE_OnQTEStart;
        QTE.instance.OnQTEEnd -= Instance_OnQTEEnd;
    }

    private void SpawnManager_OnBossFightFinished()
    {
        gunJamProbability = 0.0075f;
    }

    private void SpawnManager_OnMiniBossFightStarted()
    {
        gunJamProbability = 0;
    }

    private void Instance_OnQTEEnd(IQTECaller caller)
    {
        isPlayerInQTE = false;
    }

    private void QTE_OnQTEStart()
    {
        isPlayerInQTE = true;
    }

    private void InputManager_OnReloadAction()
    {
        if (!reloading)
        {
            currentBulletNum = magCount;
            OnSuccessfulReload?.Invoke(magCount);
            reloading = true;
            isJammed = false;
        }
    }

    private void InputManager_OnShootAction()
    {
        if (GameManager.Instance.gamePaused)
            return;


        if(isPlayerInQTE)
        {
            return;
        }

        if(reloading)
        {
            return;
        }

        if (isJammed)
        {
            SoundManager.Instance.PlayNoAmmoSound();
            OnWeaponJammed?.Invoke();
            return;
        }

        if (currentBulletNum > 0)
        {
            isJammed = gunJamProbability > UnityEngine.Random.Range(0, 1f);

            TryShootBullet();

            if(isJammed)
            {
                OnWeaponJammed?.Invoke();
            }

            if (!isJammed)
            {
                ShootBullet();
                sparksParticles.Play();
                isShooting = true;
                timerToSpawnSmoke = timeToSpawnSmoketotal;

                SoundManager.Instance.PlayShootSound();
                OnSuccessfulShoot?.Invoke(currentBulletNum,magCount);
            }
        }
        else
        {
            SoundManager.Instance.PlayNoAmmoSound();
        }
    }

    private void Update()
    {
        if (reloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                reloadTimer = reloadTimerTotal;
                reloading = false;
            }
        }

        if (isShooting)
        {
            timerToSpawnSmoke -= Time.deltaTime;
            if (timerToSpawnSmoke <= 0)
            {
                isShooting = false;
                var velocityOverLifetime = barrellParticles.velocityOverLifetime;
                velocityOverLifetime.x = playerPos.localScale.x > 0 ? 1 : -1;

                var forceOverLifetime = barrellParticles.forceOverLifetime;
                forceOverLifetime.x = playerPos.localScale.x > 0 ? 1 : -1;

                barrellParticles.Play();
                timerToSpawnSmoke = timeToSpawnSmoketotal;
            }
        }
        
    }

    private void ShootBullet()
    {

        RaycastHit2D raycastHit2D = TryShootBullet();
        if (raycastHit2D)
        {
            OnBulletHit?.Invoke(raycastHit2D.collider.gameObject, raycastHit2D.collider.gameObject.layer, raycastHit2D.point);
        }
        currentBulletNum--;
    }

    private RaycastHit2D TryShootBullet()
    {
        Vector3 shootDirection = playerPos.localScale.x > 0 ? transform.right : transform.right * -1;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(shootStartingPoint.position, shootDirection, shootDistance, shootableLayerMasks);
        if (raycastHit2D)
        {
            if (raycastHit2D.collider.gameObject.TryGetComponent<Teleporter>(out Teleporter teleporter))
            {
                isJammed = true;
            }
        }
        return raycastHit2D;
    }
}
