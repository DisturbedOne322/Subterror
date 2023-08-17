using System.Collections;
using UnityEngine;

public class DangerSign : MonoBehaviour
{
    [SerializeField]
    private GameObject dangerSign;

    private PlayerMovement player;

    private bool isInDanger = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        Ghost.OnDangerTrackGhost += Ghost_OnDangerTrackGhost;
        Ghost.OnStopDangerTrack += Ghost_OnStopDangerTrack;

        dangerSign.SetActive(false);    
    }

    private void Ghost_OnStopDangerTrack()
    {
        isInDanger = false;
        dangerSign.SetActive(false);
    }

    private void Ghost_OnDangerTrackGhost(Transform obj)
    {
        dangerSign.SetActive(true);
        isInDanger = true;
        StartCoroutine(TrackGhost(obj));
    }

    private void OnDestroy()
    {
        Ghost.OnDangerTrackGhost -= Ghost_OnDangerTrackGhost;
        Ghost.OnStopDangerTrack -= Ghost_OnStopDangerTrack;
    }


    private IEnumerator TrackGhost(Transform ghostTransform)
    {
        while (isInDanger)
        {
            dangerSign.transform.position = player.transform.position;
            Vector3 lookDirection = ghostTransform.position - transform.position;
            float angle = Vector3.SignedAngle(dangerSign.transform.up, lookDirection.normalized, Vector3.forward);
            dangerSign.transform.Rotate(0, 0, angle);
            yield return null;
        }
    }
}
