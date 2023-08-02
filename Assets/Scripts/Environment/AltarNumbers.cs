using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarNumbers : MonoBehaviour
{
    [SerializeField]
    private Sprite[] numberSprites;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private GameObject[] bodyParts;
    [SerializeField]
    private GameObject[] picturesOnAltar;

    [SerializeField]
    private GameObject[] pictures;

    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        Teleport.OnTeleportedPlayer += Teleport_OnTeleportedPlayer;
    }

    private void Teleport_OnTeleportedPlayer(Vector3 temp)
    {
        if (i >= numberSprites.Length)
            return;
        picturesOnAltar[i].SetActive(true);
        bodyParts[i].SetActive(true);
        pictures[i].SetActive(true);
        spriteRenderer.sprite = numberSprites[i];
        i++;
    }
}
