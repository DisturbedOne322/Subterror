using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MagicHole : MonoBehaviour
{
    private VisualEffect magicHoleVFX;

    private const string CIRCLE_SIZE = "CircleSize";
    private const string DISTANCE_TO_PLAYER = "DistanceToPlayer";
    private const string CONE_HEIGHT = "ConeHeight";

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip createMagicHoleSound;

    private float minCirlceSize = 0;
    private float currentCircleSize = 0;
    private float maxCircleSize = 1;

    private PlayerMovement player;

    private bool magicHoleCreated = false;

    private float minDistance = 0;
    private float distanceToPlayer = 0;
    private float currentDistance = 0;

    private float coneHeight;

    private float distanceVelocity;
    private float smDampTime = 1f;

    private float attackDuration = 5;

    private float pullingForce = 4.5f;

    public static event Action OnPlayerSucked;

    private void Awake()
    {
        magicHoleVFX = GetComponent<VisualEffect>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        MageBoss.OnFightFinished += MageBoss_OnFightFinished;
    }

    private void OnDestroy()
    {
        MageBoss.OnFightFinished -= MageBoss_OnFightFinished;
    }

    private void MageBoss_OnFightFinished()
    {
        attackDuration = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(magicHoleCreated)
        {
            attackDuration -= Time.deltaTime;
            if (attackDuration < 0)
            {
                //finish attack
                StartCoroutine(CloseMagicHole());
                return;
            }
            OnPlayerSucked?.Invoke();
            Vector2 vectorFromPlayerToHole = (transform.position - player.transform.position).normalized;
            player.transform.Translate(vectorFromPlayerToHole * pullingForce * Time.deltaTime);

            distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            currentDistance = magicHoleVFX.GetFloat(DISTANCE_TO_PLAYER);

            coneHeight = currentDistance / 5;
            magicHoleVFX.SetFloat(CONE_HEIGHT, coneHeight);

            magicHoleVFX.SetFloat(DISTANCE_TO_PLAYER, Mathf.SmoothDamp(currentDistance, distanceToPlayer, ref distanceVelocity, smDampTime));
        }
    }

    public void Initialize(float duration)
    {
        currentDistance = minDistance;
        currentCircleSize = minCirlceSize;
        attackDuration = duration;
        magicHoleCreated = false;
        audioSource.PlayOneShot(createMagicHoleSound);
        StartCoroutine(CreateMagicHole());
    }   
    
    IEnumerator CreateMagicHole()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            currentCircleSize = i;
            magicHoleVFX.SetFloat(CIRCLE_SIZE, currentCircleSize);
            yield return new WaitForSeconds(0.01f);
        }
        //in case of floating point error
        currentCircleSize = maxCircleSize;
        magicHoleVFX.SetFloat(CIRCLE_SIZE, currentCircleSize);

        magicHoleCreated = true;
        audioSource.Play();
    }

    IEnumerator CloseMagicHole()
    {
        for (float i = 1; i > 0; i -= 0.01f)
        {
            currentCircleSize = i;
            magicHoleVFX.SetFloat(CIRCLE_SIZE, currentCircleSize);

            currentDistance = magicHoleVFX.GetFloat(DISTANCE_TO_PLAYER);
            magicHoleVFX.SetFloat(DISTANCE_TO_PLAYER, Mathf.SmoothDamp(currentDistance, minDistance, ref distanceVelocity, smDampTime));
            
            coneHeight = currentDistance / 5;
            magicHoleVFX.SetFloat(CONE_HEIGHT, coneHeight);

            yield return new WaitForSeconds(0.01f);
        }
        //in case of floating point error
        currentCircleSize = minCirlceSize;
        magicHoleVFX.SetFloat(CIRCLE_SIZE, currentCircleSize);
        audioSource.Stop();
        magicHoleCreated = false;
    }
}
