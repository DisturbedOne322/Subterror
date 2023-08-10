using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCracked : MonoBehaviour
{
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoundManager.Instance.PlayFloorCracked();
    }
}
