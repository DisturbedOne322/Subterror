using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : MonoBehaviour
{
    private PlayerMovement player;
    private PlayerHealth playerHealth;
    private Rigidbody2D playerRb;

    private Vector2 throwbackVector;
    private float throwbackForce = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (player.transform.position.x > transform.position.x)
            {
                throwbackVector = new Vector2(1,0);
            }
            else
            {
                throwbackVector = new Vector2(-1,0);
            }

            playerHealth.TakeDamage(1);
            playerRb.AddForce(throwbackVector * throwbackForce, ForceMode2D.Impulse);
        }
    }
}
