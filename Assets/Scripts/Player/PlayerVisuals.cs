using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private Transform pointingDirection;

    private PlayerMovement player;

    public static float PlayerScale;

    private bool playerDead = false;

    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        player.GetComponentInChildren<PlayerHealth>().OnDeath += Player_OnPlayerDied;
    }

    private void OnDestroy()
    {
        player.GetComponentInChildren<PlayerHealth>().OnDeath -= Player_OnPlayerDied;
    }

    private void Player_OnPlayerDied()
    {
        playerDead = true;
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
