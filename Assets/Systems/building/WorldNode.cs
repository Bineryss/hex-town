using System.Collections.Generic;
using Sirenix.OdinInspector;
using Systems.Grid;
using UnityEngine;

public class WorldNode : MonoBehaviour, INode
{
    public WorldTile worldTile;

    [Header("Debug Info")]
    [ShowInInspector, ReadOnly]
    public ResourceType ResourceType => worldTile.resourceType;
    [ShowInInspector, ReadOnly]
    public int ResourceAmount => worldTile.resourceAmount * 1; //TODO modify based on improvements, etc.
    [ShowInInspector, ReadOnly]
    public HexCoordinate Position { get; set; }
    [ShowInInspector, ReadOnly]
    public bool IsWalkable => worldTile.isWalkable;
    [ShowInInspector, ReadOnly]
    public int MovementCost => worldTile.movementCost;

    private GameObject tile;
    private WorldTile currentTile;
    void Start()
    {
        if (worldTile != null)
        {
            CreateTile();
        }
    }

    void Update()
    {
        if (worldTile.Equals(currentTile)) return;

        CreateTile();
        currentTile = worldTile;
    }

    private void CreateTile()
    {
        if (tile != null) Destroy(tile);
        int rotationSteps = Random.Range(0, 6);
        Quaternion rotation = Quaternion.Euler(0, rotationSteps * 60f, 0);
        tile = Instantiate(worldTile.prefab, transform.position, rotation, transform);
    }

    public List<INode> Neighbors(Dictionary<HexCoordinate, INode> allNodes)
    {
        List<INode> neighbors = new();

        foreach (var dir in HexMetrics.Directions)
        {
            HexCoordinate neighborPos = Position + dir;
            if (allNodes.TryGetValue(neighborPos, out INode neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public void Select()
    {
        tile.transform.position += Vector3.up * 0.1f;
    }

    public void Deselect()
    {
        tile.transform.position -= Vector3.up * 0.1f;
    }
}
