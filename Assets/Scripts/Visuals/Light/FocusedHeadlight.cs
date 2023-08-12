using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FocusedHeadlight : MonoBehaviour
{
    private Light2D light2D;
    [SerializeField]
    private Light2D focusedLight2D;
    private float defaultLightIntensity;
    private readonly float focusedLightIntensity = 4f;
    private readonly float brokenLightIntensity = 0.3f;

    //public static event Action<Ghost> OnGhostFound;
    //public static event Action<TentacleStateManager> OnTentacleFound;
    //public event Action<ExecutionerVisuals> OnExecutionerFound;

    //public static event Action

    private float focusSpeed = 1;
    private float lightSmDampVelocity;

    private bool focusedLightEnabled = false;

    private readonly float focusedLightCapacity = 1f;
    private float currentFocusedLightCapacity;
    public float CurrentFocusedLightCapacity
    {
        get { return currentFocusedLightCapacity; }
    }
    private float focusedLightSpendRate = 0.15f;//0.2f;
    private readonly float focusedLightRestoreRate = 0.075f;


    private readonly float brokenHeadlightProbability = 0.03f; 
   
    private RaycastHit2D[] reactToLighEnemiesHits;

    private readonly float boxLength = 10f;
    private readonly float boxHeight = 1.5f;

    [SerializeField]
    private LayerMask reactableToLightMask;

    private Vector2[] focusedHeadlightBoxPoints;

    //increase headlightBox with time
    private float headlightBoxSizeMultiplier = 0.4f;
    private float growSpeed = 0.25f;

    private bool automaticLight = false;

    private PlayerMovement player;
    private IDamagable health;

    [SerializeField]
    private EnemySpawnManager enemySpawnManager;
    // Start is called before the first frame update
    void Start()
    {
        light2D = GetComponent<Light2D>();
        defaultLightIntensity = light2D.intensity;
        InvokeRepeating("TryToBrakeHeadlight", 0, 0.5f);
        currentFocusedLightCapacity = focusedLightCapacity;
        focusedHeadlightBoxPoints = new Vector2[3];
        reactToLighEnemiesHits = new RaycastHit2D[10];
        enemySpawnManager.OnMiniBossFightStarted += EnemySpawnManager_OnBossFightStarted;
        enemySpawnManager.OnBossFightFinished += EnemySpawnManager_OnBossFightFinished;

        player = GameManager.Instance.GetPlayerReference();
        player.OnEnterExitSafeZone += Player_OnEnterExitSafeZone;
        InputManager.Instance.OnHeadlightAction += Instance_OnHeadlightAction;


        health = GetComponentInParent<IDamagable>();
        health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath()
    {
        if (focusedLightEnabled)
        {
            focusedLightEnabled = false;
            automaticLight = true;
            SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
        }
    }

    private void OnDestroy()
    {
        enemySpawnManager.OnMiniBossFightStarted -= EnemySpawnManager_OnBossFightStarted;
        enemySpawnManager.OnBossFightFinished -= EnemySpawnManager_OnBossFightFinished;

        player.OnEnterExitSafeZone -= Player_OnEnterExitSafeZone;
        InputManager.Instance.OnHeadlightAction -= Instance_OnHeadlightAction;
        
        health.OnDeath -= Health_OnDeath;
    }


    private void Player_OnEnterExitSafeZone(bool inSafety)
    {
        if (inSafety)
        {
            automaticLight = true;
            currentFocusedLightCapacity = 1;
            if (focusedLightEnabled)
            {
                focusedLightEnabled = false;
                SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
            }
        }
        else
        {
            automaticLight = false;
        }
    }

    

    private void EnemySpawnManager_OnBossFightFinished()
    {
        focusedLightSpendRate = 0.15f;
        focusedLightEnabled = false;
        automaticLight = false;
        SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
    }

    private void EnemySpawnManager_OnBossFightStarted()
    {
        focusedLightSpendRate = 0f;
        focusedLightEnabled = true;
        automaticLight = true;
        SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
    }

    private void FixedUpdate()
    {
        if (focusedLightEnabled)
        {
            reactToLighEnemiesHits = Physics2D.BoxCastAll(transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right, 0, reactableToLightMask);
            for(int i = 0;i < reactToLighEnemiesHits.Length; i++)
            {
                reactToLighEnemiesHits[i].collider.gameObject.GetComponent<IReactToLight>().ReactToLight();
            }
            //ghostHit = Physics2D.BoxCast(transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right, 0, ghostLayerMask);
            //if (ghostHit)
            //{
            //    OnGhostFound?.Invoke(ghostHit.collider.gameObject.GetComponent<Ghost>());
            //}

            //tentaclesHits = Physics2D.BoxCastAll(transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right, 0, tentacleLayerMask);
            //for(int i = 0; i <  tentaclesHits.Length; i++)
            //{
            //    OnTentacleFound?.Invoke(tentaclesHits[i].collider.gameObject.GetComponent<TentacleStateManager>());
            //}

            //executionerHits = Physics2D.BoxCastAll(transform.position, new Vector2(boxLength, boxHeight), transform.parent.parent.rotation.eulerAngles.z, Vector2.right, 0, executionerLayerMask);
            //for (int i = 0; i < executionerHits.Length; i++)
            //{
            //    OnExecutionerFound?.Invoke(executionerHits[i].collider.gameObject.GetComponent<ExecutionerVisuals>());
            //}
        }
    }
    private void Instance_OnHeadlightAction()
    {
        if (automaticLight)
            return;

        if(!focusedLightEnabled)
        {
            if(currentFocusedLightCapacity < 0.2f)
                return;
        }
        focusedLightEnabled = !focusedLightEnabled;
        SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
    }

    public Vector2[] GetFocusedHeadlightBoxPoints()
    {
        return focusedHeadlightBoxPoints;
    }

    private void TryToBrakeHeadlight()
    {
        if (!focusedLightEnabled)
        {
            return;
        }
        if (brokenHeadlightProbability > UnityEngine.Random.Range(0, 1f))
        {
            focusedLight2D.intensity = brokenLightIntensity;
            SoundManager.Instance.PlayBrokenHeadlightSound();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (focusedLightEnabled)
        {
            focusedLight2D.intensity = Mathf.SmoothDamp(focusedLight2D.intensity, focusedLightIntensity, ref lightSmDampVelocity, focusSpeed);
            currentFocusedLightCapacity -= focusedLightSpendRate * Time.deltaTime;

            headlightBoxSizeMultiplier = Mathf.Clamp01(headlightBoxSizeMultiplier + Time.deltaTime * growSpeed);

            focusedHeadlightBoxPoints[0] = BoxCastDrawer.GetTopLeftOfBox(transform.position, new Vector2(boxLength, boxHeight) * headlightBoxSizeMultiplier, transform.parent.parent.rotation.eulerAngles.z);
            focusedHeadlightBoxPoints[1] = BoxCastDrawer.GetTopRightOfBox(transform.position, new Vector2(boxLength, boxHeight) * headlightBoxSizeMultiplier, transform.parent.parent.rotation.eulerAngles.z);
            focusedHeadlightBoxPoints[2] = BoxCastDrawer.GetBottomRightOfBox(transform.position, new Vector2(boxLength, boxHeight) * headlightBoxSizeMultiplier, transform.parent.parent.rotation.eulerAngles.z);
        }
        else
        {
            focusedLight2D.intensity = Mathf.SmoothDamp(focusedLight2D.intensity, defaultLightIntensity, ref lightSmDampVelocity, focusSpeed);
            currentFocusedLightCapacity += focusedLightRestoreRate * Time.deltaTime;
            headlightBoxSizeMultiplier = 0.4f;
            focusedHeadlightBoxPoints[0] = Vector2.zero;
            focusedHeadlightBoxPoints[1] = Vector2.zero;
            focusedHeadlightBoxPoints[2] = Vector2.zero;
        }

        if (currentFocusedLightCapacity < 0)
        {
            focusedLightEnabled = false;
            SoundManager.Instance.PlayFocusedLightSound(focusedLightEnabled);
        }

        currentFocusedLightCapacity = Mathf.Clamp(currentFocusedLightCapacity, 0, focusedLightCapacity);
    }
}
