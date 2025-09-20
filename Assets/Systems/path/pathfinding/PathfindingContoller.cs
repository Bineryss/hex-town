using System.Collections.Generic;
using System.Linq;
using Systems.Grid;
using UnityEngine;

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

    public void Update()
    {
        //add click to world
    }

}