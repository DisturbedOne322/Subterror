using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Dissolve))]
public class DissolveOnCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetComponent<Dissolve>().SetDissolveTickRate(2);
        GetComponent<Dissolve>().StartDissolving();
    }
}
