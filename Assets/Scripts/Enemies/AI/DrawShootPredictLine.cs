using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawShootPredictLine : MonoBehaviour
{
    [SerializeField]
    private Transform parentGO;

    [SerializeField]
    private Transform lineRendererStartPos;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField, Min(0)]
    private float lineDistance;


    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, lineRendererStartPos.position);

        Vector3 linePos2 = transform.position + (parentGO.transform.localScale.x * lineDistance * transform.right);

        lineRenderer.SetPosition(1, linePos2);
    }
}
