using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class HellHoundVisuals : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private HellHound hellHound;

    private const string WALK_ANIM = "PlayerSpotted";
    private const string RUN_ANIM = "OnAggressiveStateChange";
    private const string ATTACK_ANIM = "PlayerInAttackRange";

    [SerializeField]
    private PlayerInRange playerDetection;
    private bool playerFound = false;

    private PlayerMovement player;

    [SerializeField]
    private HitIndicatorsSO hitIndicatorsSO;
    private ParticleSystem[] particleSpawnerOnPlayerHitWithClaw;

    [SerializeField]
    private Dissolve dissolve;

    private void Awake()
    {
        hellHound = GetComponent<HellHound>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();

        playerDetection.OnPlayerInRange += PlayerDetection_OnPlayerInRange;
        hellHound.OnAggressiveStateChange += HellHound_OnAggressiveStateChange;
        hellHound.OnHellHoundAttack += HellHound_OnHellHoundAttack;
        hellHound.OnSuccessfulHit += HellHound_OnSuccessfulHit;

        //pre instantiate particles on player hit with claws
        particleSpawnerOnPlayerHitWithClaw = new ParticleSystem[hitIndicatorsSO.ClawCutHitIndicatorArray.Length];
        for (int i = 0; i < hitIndicatorsSO.ClawCutHitIndicatorArray.Length; i++)
        {
            particleSpawnerOnPlayerHitWithClaw[i] = Instantiate(hitIndicatorsSO.ClawCutHitIndicatorArray[i]);
            particleSpawnerOnPlayerHitWithClaw[i].gameObject.transform.SetParent(player.transform, false);
            particleSpawnerOnPlayerHitWithClaw[i].Stop();
        }

        StartCoroutine(StartAnimationWithRandomDelay());
    }

    private IEnumerator StartAnimationWithRandomDelay()
    {
        animator.enabled = false;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, 1f));
        animator.enabled = true;
    }

    private void OnDestroy()
    {
        playerDetection.OnPlayerInRange -= PlayerDetection_OnPlayerInRange;
        hellHound.OnAggressiveStateChange -= HellHound_OnAggressiveStateChange;
        hellHound.OnHellHoundAttack -= HellHound_OnHellHoundAttack;
        hellHound.OnSuccessfulHit -= HellHound_OnSuccessfulHit;
    }

    private void HellHound_OnSuccessfulHit()
    {
        int playerHitWithClawRandomIndicatorIndex = Random.Range(0, particleSpawnerOnPlayerHitWithClaw.Length);
        particleSpawnerOnPlayerHitWithClaw[playerHitWithClawRandomIndicatorIndex].transform.position = player.transform.position;
        particleSpawnerOnPlayerHitWithClaw[playerHitWithClawRandomIndicatorIndex].Play();
    }

    private void HellHound_OnHellHoundAttack()
    {
        animator.SetTrigger(ATTACK_ANIM);    
    }

    private void HellHound_OnAggressiveStateChange()
    {
        animator.SetBool(RUN_ANIM, true);
    }

    private void PlayerDetection_OnPlayerInRange(PlayerMovement player)
    {
        animator.SetBool(WALK_ANIM, true);
        playerFound = true;
    }

    private void Update()
    {
        if(player!=null && playerFound)
        {
            var scale = transform.localScale;
            scale.x = transform.position.x > player.transform.position.x? 5 : -5;
            transform.localScale = scale;

            animator.SetFloat("RunSpeed", dissolve.DissolveOnDeath());
        }
    }
}
