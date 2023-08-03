using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public event Action OnBossFightStarted;

    public static GameManager Instance;

    [SerializeField]
    private ParticleHitSO particleHitSO;
    private ParticleSystem[] particleSpawnerOnEnemyHit;

    [SerializeField]
    private CinemachineVirtualCamera baseCamera;
    [SerializeField]
    private CinemachineVirtualCamera aimCamera;

    [SerializeField]
    private PlayerMovement player;

    [SerializeField]
    private Ghost ghost;

    [SerializeField]
    private Teleporter teleporter;

    [SerializeField]
    private GameObject boss;

    public bool gamePaused { get; private set; }

    public Vector2 BottomLeftScreenBoundaries
    {
        get { return Camera.main.ScreenToWorldPoint(new Vector2(0, 0)); }
    }
    public Vector2 TopRightScreenBoundaries
    {
        get { return Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight)); }
    }

    public enum AttackType
    {
        ClawCut,
        ArrowHit
    }

    public enum PlayerHealthStatus
    {
        HighHP,
        MidHP,
        LowHP
    }

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
        gamePaused = false;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
        Shoot.OnBulletHit += Shoot_OnBulletHit;
        //pre instantiate bullet hit particles
        particleSpawnerOnEnemyHit = new ParticleSystem[particleHitSO.particleArray.Length];
        for (int i = 0; i < particleHitSO.particleArray.Length; i++)
        {
            particleSpawnerOnEnemyHit[i] = Instantiate(particleHitSO.particleArray[i]);
        }

        InputManager.Instance.OnFocusActionStarted += Instance_OnFocusActionStarted;
        InputManager.Instance.OnFocusActionEnded += Instance_OnFocusActionEnded;

        PauseMenuManager.OnGamePaused += PauseMenuManager_OnGamePaused;

        InitiateBossfight.OnBossFightInitiated += InitiateBossfight_OnBossFightInitiated;
    }

    private void OnDestroy()
    {
        Shoot.OnBulletHit -= Shoot_OnBulletHit;

        InputManager.Instance.OnFocusActionStarted -= Instance_OnFocusActionStarted;
        InputManager.Instance.OnFocusActionEnded -= Instance_OnFocusActionEnded;

        PauseMenuManager.OnGamePaused -= PauseMenuManager_OnGamePaused;

        InitiateBossfight.OnBossFightInitiated -= InitiateBossfight_OnBossFightInitiated;
    }

    public void PauseMenuManager_OnGamePaused(bool paused)
    {
        gamePaused = paused;
        Cursor.visible = paused;
        Time.timeScale = gamePaused ? 0 : 1;
    }
    private void InitiateBossfight_OnBossFightInitiated()
    {
        boss.gameObject.SetActive(true);
        OnBossFightStarted?.Invoke();
    }

    public PlayerMovement GetPlayerReference()
    {
        return player;
    }

    public Ghost GetGhostReference()
    {
        return ghost;
    }

    public Teleporter GetTeleporterReference()
    {
        return teleporter;
    }

    private void Instance_OnFocusActionStarted()
    {
        baseCamera.gameObject.SetActive(false);
        aimCamera.gameObject.SetActive(true);
    }

    private void Instance_OnFocusActionEnded()
    {
        baseCamera.gameObject.SetActive(true);
        aimCamera.gameObject.SetActive(false);
    }

    private void Shoot_OnBulletHit(GameObject objectHit, LayerMask layerMask, Vector3 pos)
    {
        for (int i = 0; i < particleHitSO.layerMaskArray.Length; i++)
        {
            if (((1 << layerMask) & particleHitSO.layerMaskArray[i]) != 0)
            {
                //works either way if it hit any of the layers in array
                particleSpawnerOnEnemyHit[i].transform.position = pos;
                particleSpawnerOnEnemyHit[i].Play();

                if(objectHit.TryGetComponent<IDamagable>(out IDamagable damagable))
                {
                    damagable.TakeDamage(1);
                }

                if(objectHit.TryGetComponent<WeakPoint>(out WeakPoint weakPoint))
                {
                    weakPoint.GetDamage();
                }

                //if the object hit is derived from enemy class
                if (objectHit.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    if (enemy.TryGetComponent<Ghost>(out Ghost ghost))
                    {
                        if (!ghost.Vulnerable)
                        {
                            particleSpawnerOnEnemyHit[i].Stop();
                            return;
                        }
                    }
                    
                    enemy.GetDamage(1);
                    BoxCollider2D hitBox = enemy.GetComponent<BoxCollider2D>();
                    Vector3 particleSpawnPos = new Vector3(UnityEngine.Random.Range(0, hitBox.size.x), UnityEngine.Random.Range(0, hitBox.size.y),0);

                    particleSpawnerOnEnemyHit[i].transform.position = objectHit.transform.position + particleSpawnPos;
                }
            }
        }
    }
}
