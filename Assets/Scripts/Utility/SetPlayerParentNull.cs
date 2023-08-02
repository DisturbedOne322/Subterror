using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerParentNull : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GetPlayerReference().transform.parent = null;
        }
    }
}
