using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    private SurfaceEffector2D effector;

    private float risingSpeed = 18;
    private float descendSpeed = 0;

    private void Awake()
    {
        effector = GetComponent<SurfaceEffector2D>();   
    }

    // Update is called once per frame
    void Update()
    {
        float playerXMovement = InputManager.Instance.GetMovementDirection();

        if(playerXMovement == 0)
        {
            effector.speed = 0;
            return;
        }


        float speed = 0;

        if (transform.localScale.x > 0)
        {
            speed = playerXMovement > 0 ? descendSpeed : -risingSpeed;
        }
        else
        {
            speed = playerXMovement > 0 ? risingSpeed : descendSpeed;
        }

        effector.speed = speed;
    }
}
