using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private Transform pointingDirection;

    [SerializeField]
    private ParticleSystem slidingOnMudParticles;

    private PlayerMovement player;
    private PlayerHealth playerHealth;

    public static float PlayerScale;

    private bool playerDead = false;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerHealth.OnDeath += Player_OnPlayerDied;
        slidingOnMudParticles.Stop();
    }

    private void OnDestroy()
    {
        playerHealth.OnDeath -= Player_OnPlayerDied;
    }

    private void Player_OnPlayerDied()
    {
        playerDead = true;
    }

    public void PlayMudSlidingParticles()
    {
        slidingOnMudParticles.Play();
    }
    public void StopMudSlidingParticles()
    {
        slidingOnMudParticles.Stop();
    }


    private void Update()
    {
        if (GameManager.Instance.gamePaused)
            return;
        if (playerDead)
            return;

        if(pointingDirection.position.x > transform.position.x)
        {
            var scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
        if(pointingDirection.position.x < transform.position.x)
        {
            var scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        PlayerScale = transform.localScale.x;
    }
}
