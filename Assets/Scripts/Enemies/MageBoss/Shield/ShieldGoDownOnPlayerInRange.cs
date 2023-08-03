using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldGoDownOnPlayerInRange : MonoBehaviour
{
    private SplineFollower splineFollower;

    private Transform playerPos;

    private float distance = 4;

    private float defaultSpeed;
    private float descendSpeed = 6;

    // Start is called before the first frame update
    void Start()
    {
        splineFollower = GetComponent<SplineFollower>();
        defaultSpeed = splineFollower.followSpeed;
        playerPos = GameManager.Instance.GetPlayerReference().transform;
    }

    private void Update()
    {
        bool inRange = transform.position.x - playerPos.position.x < distance;
        if (inRange)
        {
            Descend();
        }
        else
        {
            NormalMove();
        }
    }

    private bool descending = false;
    private bool normalMoving = false;

    private void Descend()
    {
        if (descending)
            return;

        descending = true;
        splineFollower.followSpeed = descendSpeed;
        splineFollower.direction = Spline.Direction.Backward;
        splineFollower.wrapMode = SplineFollower.Wrap.Default;
        normalMoving = false;
    }

    private void NormalMove()
    {
        if (normalMoving)
            return;

        normalMoving = true;
        splineFollower.followSpeed = defaultSpeed;
        splineFollower.direction = Spline.Direction.Forward;
        splineFollower.wrapMode = SplineFollower.Wrap.PingPong;
        descending = false;
    }
}
