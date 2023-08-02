using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCrossHair : MonoBehaviour
{
    [SerializeField]
    private LookAtMouse gun;
    [SerializeField]
    private GameObject[] crossHairSpriteArray;

    private float focusedAlpha = 1f;
    private float unfocusedAlpha = 0.5f;

    // Update is called once per frame
    void Update()
    {
        foreach (var sprite in crossHairSpriteArray)
        {
            Color tmp = sprite.GetComponent<SpriteRenderer>().color;
            float alpha;
            if (gun.Focused)
            {
                alpha = Mathf.Lerp(tmp.a, focusedAlpha, 0.2f);
            }
            else
            {
                alpha = Mathf.Lerp(tmp.a, unfocusedAlpha, 0.2f);
            }

            tmp.a = alpha;
            sprite.GetComponent<SpriteRenderer>().color = tmp;
        }
    }
}
