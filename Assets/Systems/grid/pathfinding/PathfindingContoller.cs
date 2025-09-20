using System.Collections.Generic;
using System.Linq;
using Systems.Grid;
using UnityEngine;

public class PathfindingController : MonoBehaviour
{
    public HexCoordinate target;
    public HexGridGenerator generator;
    public LineRenderer lineRenderer;

    [ContextMenu("Find Path")]
    public void FindPath()
    {
        Debug.Log("Start Path finding");
        List<HexCoordinate> path = PathFinder.FindPath(generator.nodes[new(0, 0)], generator.nodes[target], generator.nodes);

        Vector3[] worldPositions = path.Select(p => generator.grid.CellToWorld(p.ToOffset())).ToArray();

        lineRenderer.positionCount = worldPositions.Length;
        lineRenderer.SetPositions(worldPositions);
    }

}