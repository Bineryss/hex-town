using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using Systems.Transport;
using UnityEngine;

public class WorldNode : SerializedMonoBehaviour, INode
{
    private TransportManager TransportManager => TransportController.Instance.Manager;
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
    public ResourceType ResourceType => ConnectedNodes.Count == 0 ? worldTile.resourceType : ConnectedNodes[0].ResourceType;
    [ShowInInspector, ReadOnly]
    public int Production;
    public List<WorldTile> ConnectableTiles => worldTile.connectableTiles;

    [Header("Transport Info")]
    [OdinSerialize]
    public List<Guid> outgoingRoutes = new();
    [OdinSerialize]
    public List<Guid> incomingRoutes = new();
    [OdinSerialize, ReadOnly]
    public List<ResourceType> AcceptedInputResources => worldTile.TradeableResources;
    public Dictionary<ResourceType, int> MaxIncomingCapacity => maxCapacities;
    private bool isSelected;
    [OdinSerialize] private readonly Dictionary<ResourceType, int> maxCapacities = new();
    [SerializeField] public List<WorldNode> ConnectedNodes = new();
    [SerializeField] public bool isSubTile;




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

    public void InitializeWithSubTiles(WorldTile tile, HexCoordinate position, List<WorldNode> connectedNodes)
    {
        this.ConnectedNodes = connectedNodes;
        connectedNodes.ForEach(t => t.isSubTile = true);
        Initialize(tile, position);
    }
    private void CreateTile()
    {
        if (tile != null) Destroy(tile);
        int rotationSteps = UnityEngine.Random.Range(0, 6);
        Quaternion rotation = Quaternion.Euler(0, rotationSteps * 60f, 0);
        tile = Instantiate(worldTile.prefab, transform.position, rotation, transform);
        CalculateProduction();
        foreach (ResourceBonus bonus in worldTile.inputBonuses)
        {
            maxCapacities[bonus.input] = bonus.maxCapacity;
        }
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
        if (isSelected) return;
        transform.position += Vector3.up * 0.1f;
        isSelected = true;
    }

    public void Deselect()
    {
        if (!isSelected) return;
        transform.position -= Vector3.up * 0.1f;
        isSelected = false;
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
        float productionBonus = 0f;

        foreach (ResourceBonus bonus in worldTile.inputBonuses)
        {
            Dictionary<ResourceType, int> incomingResources = TransportManager.GetIncomingResourcesFor(incomingRoutes);
            if (incomingResources.TryGetValue(bonus.input, out int amount))
            {
                float effectiveBonus = Mathf.Min(amount, bonus.maxCapacity) * (bonus.bonusMultiplier / 100f);
                productionBonus += effectiveBonus;
            }
        }
        Production = Mathf.FloorToInt(Production * (1 + productionBonus));
    }

    public int GetAvailableProduction()
    {
        if (isSubTile)
        {
            return 0;
        }

        int totalOutput = Production;
        foreach (Guid routeId in outgoingRoutes)
        {
            TransportRoute route = TransportManager.GetRoute(routeId);
            if (route != null)
            {
                totalOutput -= route.quantity;
            }
        }

        foreach (WorldNode node in ConnectedNodes)
        {
            totalOutput += node.Production;
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
