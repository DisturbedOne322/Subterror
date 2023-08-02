using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouseInput : MonoBehaviour
{
    [SerializeField] 
    private Transform player;
    private Position mousePosition;
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 followPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        followPosition.z = 0;
        this.transform.position = followPosition;

        mousePosition = followPosition.x > player.position.x ? Position.RightOfPlayer : Position.LeftOfPlayer;
    }

    public enum Position
    {
        LeftOfPlayer,
        RightOfPlayer,
    }

    public Position MousePosition
    {
        get { return mousePosition; }
        private set { mousePosition = value; }
    }
}
