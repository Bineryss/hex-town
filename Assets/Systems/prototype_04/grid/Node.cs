using System.Collections.Generic;
using Systems.Core;
using UnityEngine;

[System.Serializable]
public class Node: INode
{
    public GameObject Instance { get; set; }
    public AxialCoordinate Position { get; set; }
    public int MovementCost { get; set; } = 1;
    public bool IsWalkable { get; set; } = true;

    public List<Node> Neighbors(Dictionary<AxialCoordinate, Node> allNodes)
    {
        List<Node> neighbors = new();

        foreach (var dir in HexMetrics.Directions)
        {
            AxialCoordinate neighborPos = Position + dir;
            if (allNodes.TryGetValue(neighborPos, out Node neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public void Select()
    {
        Instance.transform.position += Vector3.up * 0.1f;
    }

    public void Deselect()
    {
        if (Instance == null) return;
        Instance.transform.position -= Vector3.up * 0.1f;
    }

    public override string ToString()
    {
        return $"Node({Position}, Walkable: {IsWalkable})";
    }

    public List<INode> Neighbors(Dictionary<AxialCoordinate, INode> allNodes)
    {
        throw new System.NotImplementedException();
    }
}

public interface INode
{
    AxialCoordinate Position { get; }
    bool IsWalkable { get; }
    int MovementCost { get; }
    List<INode> Neighbors(Dictionary<AxialCoordinate, INode> allNodes);
    void Select();
    void Deselect();
}