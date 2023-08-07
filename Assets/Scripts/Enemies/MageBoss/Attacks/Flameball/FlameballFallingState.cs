using System;
using UnityEngine;

public class FlameballFallingState : FlameballBaseState
{
    private int damage = 1;
    private float puddleOffsetFromGround = 1.65f;
    private SpriteRenderer spriteRenderer;
    public static event Action OnFlameballHitGround;

    private bool canDealDamage = true;

    public override void EnterState(Flameball manager)
    {
        spriteRenderer = manager.spriteRenderer;
        manager.audioSource.Play();
        Color currentAlpha = spriteRenderer.color;
        currentAlpha.a = 1;
        spriteRenderer.color = currentAlpha;
        //random spawn;
        //idea is to have a separate stage for boss fight with a new camera. Flameball spawns in this stage close to player
    }

    public override void UpdateState(Flameball manager)
    {
        manager.FallDown();
    }

    public override void OnTriggerStay2D(Flameball manager, Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (!canDealDamage)
                return;

            manager.audioSource.Stop();
            manager.playerHealth.TakeDamage(damage);
            manager.gameObject.SetActive(false);
            canDealDamage = false;
        }
        else
        {
            //spawn on the ground that was hit
            manager.audioSource.Stop();
            if (manager.audioSource.enabled)
            {
                manager.audioSource.PlayOneShot(
                    manager.fallSoundArray[
                        UnityEngine.Random.Range(0, manager.fallSoundArray.Length)
                        ]);
            }

            manager.transform.position = new Vector2(manager.transform.position.x, collision.gameObject.transform.position.y + puddleOffsetFromGround);
            manager.SwitchState(manager.puddleState);
            OnFlameballHitGround?.Invoke();
        }
    }
}
