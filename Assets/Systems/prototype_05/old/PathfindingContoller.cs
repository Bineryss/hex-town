using System.Collections.Generic;
using System.Linq;
using Systems.Core;
using Systems.Prototype_04;
using UnityEngine;
namespace Systems.Prototype_05
{
    public class PathfindingController : MonoBehaviour
    {
        public AxialCoordinate target;
        public HexGridGenerator generator;
        public SmoothLineRenderer smoothLineRenderer;

        [ContextMenu("Find Path")]
        public void FindPath()
        {
            List<AxialCoordinate> path = PathFinder.FindPath(generator.nodes[new(0, 0)], generator.nodes[target], generator.nodes);

            Vector3[] worldPositions = path.Select(p => generator.layout.AxialToWorld(p)).Select(v => new Vector3(v.x, 0.3f, v.z)).ToArray();
            smoothLineRenderer.points = worldPositions.ToList();
        }

        public List<AxialCoordinate> FindPath(INode a, INode b)
        {
            List<AxialCoordinate> path = PathFinder.FindPath(generator.nodes[a.Position], generator.nodes[b.Position], generator.nodes);
            if (smoothLineRenderer == null) return path;

            Vector3[] worldPositions = path.Select(p => generator.layout.AxialToWorld(p)).Select(v => new Vector3(v.x, 0.3f, v.z)).ToArray();
            smoothLineRenderer.points = worldPositions.ToList();

            return path;
        }

    }
}