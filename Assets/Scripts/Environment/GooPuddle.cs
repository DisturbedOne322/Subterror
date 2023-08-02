using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GooPuddle : MonoBehaviour
{
    [SerializeField]
    private float damageCD;

    private float lastDamageTime;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(Time.time> lastDamageTime + damageCD)
            {
                collision.GetComponent<PlayerHealth>().TakeDamage(1);
                lastDamageTime = Time.time;
            }
        }
    }
}
