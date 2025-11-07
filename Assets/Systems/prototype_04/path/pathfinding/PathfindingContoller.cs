using System.Collections.Generic;
using System.Linq;
using Systems.Prototype_04.Grid;
using UnityEngine;
namespace Systems.Prototype_04
{
    public class PathfindingController : MonoBehaviour
    {
        public HexCoordinate target;
        public HexGridGenerator generator;
        public SmoothLineRenderer smoothLineRenderer;

        [ContextMenu("Find Path")]
        public void FindPath()
        {
            List<HexCoordinate> path = PathFinder.FindPath(generator.nodes[new(0, 0)], generator.nodes[target], generator.nodes);

            Vector3[] worldPositions = path.Select(p => generator.grid.CellToWorld(p.ToOffset())).Select(v => new Vector3(v.x, 0.3f, v.z)).ToArray();
            smoothLineRenderer.points = worldPositions.ToList();
        }

        public List<HexCoordinate> FindPath(INode a, INode b)
        {
            List<HexCoordinate> path = PathFinder.FindPath(generator.nodes[a.Position], generator.nodes[b.Position], generator.nodes);
            if (smoothLineRenderer == null) return path;

            Vector3[] worldPositions = path.Select(p => generator.grid.CellToWorld(p.ToOffset())).Select(v => new Vector3(v.x, 0.3f, v.z)).ToArray();
            smoothLineRenderer.points = worldPositions.ToList();

            return path;
        }

    }
}