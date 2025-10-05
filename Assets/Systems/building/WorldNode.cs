using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using Systems.Transport;
using UnityEngine;

public class WorldNode : SerializedMonoBehaviour, INode
{
    private TransportManager TransportManager => TransportManager.Instance;
    public WorldTile worldTile;

    [Header("Debug Info")]
    [ShowInInspector, ReadOnly]
    public HexCoordinate Position { get; set; }
    [ShowInInspector, ReadOnly]
    public bool IsWalkable => worldTile.isWalkable;
    [ShowInInspector, ReadOnly]
    public int MovementCost => worldTile.movementCost;

    [Header("Resource Info")]
    [ShowInInspector, ReadOnly]
    public ResourceType ResourceType => worldTile.resourceType;
    [ShowInInspector, ReadOnly]
    public int Production;

    [Header("Transport Info")]
    [OdinSerialize]
    public List<Guid> outgoingRoutes = new();
    [OdinSerialize]
    public List<Guid> incomingRoutes = new();
    [OdinSerialize, ReadOnly]
    public List<ResourceType> AcceptedInputResources => worldTile.TradeableResources;




    private GameObject tile;
    private WorldTile currentTile;
    void Awake()
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


    public void Initialize(WorldTile tile, HexCoordinate position)
    {
        worldTile = tile;
        Position = position;
        Production = worldTile.resourceAmount;
        outgoingRoutes.Clear();
        incomingRoutes.Clear();
        CalculateProduction();
    }

    private void CreateTile()
    {
        if (tile != null) Destroy(tile);
        int rotationSteps = UnityEngine.Random.Range(0, 6);
        Quaternion rotation = Quaternion.Euler(0, rotationSteps * 60f, 0);
        tile = Instantiate(worldTile.prefab, transform.position, rotation, transform);
        CalculateProduction();
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


    public void AddOutgoingRoute(Guid routeId)
    {
        if (!outgoingRoutes.Contains(routeId))
            outgoingRoutes.Add(routeId);
    }

    public void AddIncomingRoute(Guid routeId)
    {
        if (incomingRoutes.Contains(routeId)) return;

        incomingRoutes.Add(routeId);
        CalculateProduction();
    }

    public void CalculateProduction()
    {
        int newProduction = worldTile.resourceAmount;

        foreach (ResourceBonus bonus in worldTile.inputBonuses)
        {
            Dictionary<ResourceType, int> incomingResources = TransportManager.GetIncomingResourcesFor(incomingRoutes);
            if (incomingResources.TryGetValue(bonus.input, out int amount))
            {
                newProduction = Mathf.CeilToInt(newProduction * (bonus.bonusMultiplier * amount)) + worldTile.resourceAmount;
            }
        }
        Production = newProduction;
    }

    public int GetAvailableProduction()
    {
        int totalOutput = Production;
        foreach (Guid routeId in outgoingRoutes)
        {
            TransportRoute route = TransportManager.GetRoute(routeId);
            if (route != null)
            {
                totalOutput -= route.quantity;
            }
        }

        return totalOutput;
    }

    public void RemoveRoute(Guid routeId)
    {
        outgoingRoutes.Remove(routeId);
        incomingRoutes.Remove(routeId);
        CalculateProduction();
    }

}
