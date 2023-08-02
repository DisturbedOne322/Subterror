using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    private PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
    }

    private void Update()
    {
        float newScaleX = player.transform.position.x > transform.position.x ? 1 : -1;

        Vector3 newScale = transform.localScale;
        newScale.x = newScaleX;

        transform.localScale = newScale;
    }
}
