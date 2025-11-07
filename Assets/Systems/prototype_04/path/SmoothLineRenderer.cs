using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems.Prototype_04
{
    [RequireComponent(typeof(LineRenderer))]
    public class SmoothLineRenderer : MonoBehaviour
    {
        private static readonly int BaseProperty = Shader.PropertyToID("_base");

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

        public void RenderLine(List<Vector3> newPoints)
        {
            points = newPoints;
            lineVertices = PathUtils.RoundCorners(points.ToArray(), cornerRadius, arcSamples);

            lineRenderer.positionCount = lineVertices.Count;
            lineRenderer.SetPositions(lineVertices.ToArray());
        }
        public void HideLine()
        {
            lineRenderer.enabled = false;
        }

        public void ShowLine()
        {
            lineRenderer.enabled = true;
        }

        public void ChangeColor(Color color)
        {
            lineRenderer.material.SetColor(BaseProperty, color);
        }
    }
}