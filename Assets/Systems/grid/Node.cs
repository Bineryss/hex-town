using System.Collections.Generic;
using Systems.Grid;
using UnityEngine;

[System.Serializable]
public class Node
{
    public GameObject instance;
    public HexCoordinate position;
    public int movementCost = 1;
    public bool isWalkable = true;

    public List<Node> Neighbors(Dictionary<HexCoordinate, Node> allNodes)
    {
        List<Node> neighbors = new();

        foreach (var dir in HexMetrics.Directions)
        {
            HexCoordinate neighborPos = position + dir;
            if (allNodes.TryGetValue(neighborPos, out Node neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public int Distance(Node other)
    {
        return position.Distance(other.position);
    }
}