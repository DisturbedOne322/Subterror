using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCanSaveGame : MonoBehaviour
{
    public event Action<bool> CanSaveGame;

    [SerializeField]
    private float boxCastWidth;
    [SerializeField] 
    private float boxCastHeight;

    Vector2 boxCastSize;
    private RaycastHit2D[] hits;
    private WaitForSeconds delay;
    // Start is called before the first frame update
    void Start()
    {
        hits = new RaycastHit2D[10];
        boxCastSize = new Vector2(boxCastWidth, boxCastHeight);
        delay = new WaitForSeconds(0.05f);
        StartCoroutine(CheckForEnemiesNearby());
    }

    private IEnumerator CheckForEnemiesNearby()
    {
        while (true)
        {
            Physics2D.BoxCastNonAlloc(transform.position, boxCastSize, 0, Vector2.zero, hits);

            bool enemiesInRange = false;

            for(int i = 0; i < hits.Length; i++)
            {
                if (!hits[i])
                    continue;

                if (hits[i].collider.gameObject.CompareTag("Enemy")
                || hits[i].collider.gameObject.CompareTag("PlayerFake"))
                {
                    enemiesInRange = true;
                }
            }

            CanSaveGame?.Invoke(!enemiesInRange);
            yield return delay;
        }
    }
}
