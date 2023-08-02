using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float destroyTimer = 3f;

    public static event Action<LayerMask,Vector3> OnHit;

    // Update is called once per frame
    void Update()
    {
        destroyTimer -= Time.deltaTime;
        if(destroyTimer < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnHit?.Invoke(collision.gameObject.layer, transform.position);
    }
}
