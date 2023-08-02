using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerDash : MonoBehaviour
{
    [SerializeField]
    private float dashDistance;

    public void Dash()
    {
        transform.Translate(new Vector2 (dashDistance * transform.localScale.x, 0));
    }
}
