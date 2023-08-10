using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private PlayerMovement player;
    private IDamagable damagable;

    private bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        damagable = GetComponent<IDamagable>();
        damagable.OnDeath += Damagable_OnDeath;
    }

    private void Damagable_OnDeath()
    {
        isAlive = false;
    }

    private void OnEnable()
    {
        isAlive = true;
    }

    private void Update()
    {
        if(!isAlive)       
            return;
        
        float newScaleX = player.transform.position.x > transform.position.x ? 1 : -1;

        Vector3 newScale = transform.localScale;
        newScale.x = newScaleX;

        transform.localScale = newScale;
    }
}
