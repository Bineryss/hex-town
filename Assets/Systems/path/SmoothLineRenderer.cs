using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmoothLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] public List<Vector3> points;
    [SerializeField] private float cornerRadius = 0.1f;
    [SerializeField] private int arcSamples = 4;

    private List<Vector3> lineVertices;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }


    // void OnValidate()
    // {
    //     if (lineRenderer == null)
    //     {
    //         lineRenderer = GetComponent<LineRenderer>();
    //     }

    //     lineVertices = CornerSmoothing.RoundCorners(points.ConvertAll(p => p.position).ToArray(), cornerRadius, arcSamples);

    //     lineRenderer.positionCount = lineVertices.Count;
    //     lineRenderer.SetPositions(lineVertices.ToArray());
    // }

    void OnDrawGizmos()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineVertices = PathUtils.RoundCorners(points.ToArray(), cornerRadius, arcSamples);

        lineRenderer.positionCount = lineVertices.Count;
        lineRenderer.SetPositions(lineVertices.Select(el => new Vector3(el.x, 0.3f, el.z)).ToArray());
    }
}