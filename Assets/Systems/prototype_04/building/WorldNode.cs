using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using Systems.Prototype_04.Building;
using Systems.Prototype_04.Transport;
using UnityEngine;

namespace Systems.Prototype_04
{
    public class WorldNode : SerializedMonoBehaviour, INode
    {
        private TransportManager TransportManager => TransportController.Instance.Manager;
        public WorldTile worldTile;

        [Header("Debug Info")]
        [ShowInInspector, ReadOnly] public AxialCoordinate Position { get; set; }
        [ShowInInspector, ReadOnly] public bool IsWalkable => worldTile.isWalkable;
        [ShowInInspector, ReadOnly] public int MovementCost => worldTile.movementCost;

        [Header("Resource Info")]
        [ShowInInspector, ReadOnly] public ResourceType ResourceType => ConnectedNodes.Count == 0 ? worldTile.resourceType.type : ConnectedNodes[0].ResourceType;
        [ShowInInspector, ReadOnly] public Resource Resource => ConnectedNodes.Count == 0 ? worldTile.resourceType : ConnectedNodes[0].Resource;
        [ShowInInspector, ReadOnly] public float Production;
        public List<WorldTile> ConnectableTiles => worldTile.connectableTiles;

        [Header("Transport Info")]
        [OdinSerialize] public List<Guid> outgoingRoutes = new();
        [OdinSerialize] public List<Guid> incomingRoutes = new();
        [OdinSerialize, ReadOnly]
        public List<ResourceType> AcceptedInputResources
        {
            get
            {
                return ConnectedNodes.SelectMany(n => n.AcceptedInputResources).Concat(worldTile.TradeableResources).ToList();
            }
        }
        public List<ResourceBonus> InputBonuses
        {
            get
            {
                return ConnectedNodes
                .SelectMany(n => n.InputBonuses)
                .Concat(worldTile.inputBonuses)
                .GroupBy(bonus => bonus.input)
                .Select(group => new ResourceBonus { input = group.Key, maxCapacity = group.Sum(bonus => bonus.maxCapacity), bonusMultiplier = Mathf.FloorToInt(group.Sum(bonus => bonus.bonusMultiplier) / Mathf.Pow(group.Count(), 2)) })
                .ToList();
            }
        }
        public Dictionary<ResourceType, int> MaxIncomingCapacity => maxCapacities;
        private bool isSelected;
        [OdinSerialize] private Dictionary<ResourceType, int> maxCapacities = new();
        [ShowInInspector, ReadOnly] public List<WorldNode> ConnectedNodes = new();
        [ShowInInspector, ReadOnly] public bool isSubTile;
        public float CumulatedBonus => cumulatedBonus;
        private float cumulatedBonus;

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


        public void Initialize(WorldTile tile, AxialCoordinate position)
        {
            worldTile = tile;
            Position = position;
            Production = worldTile.resourceAmount + ConnectedNodes.Sum(n => n.worldTile.resourceAmount);
            outgoingRoutes.Clear();
            incomingRoutes.Clear();
            CalculateProduction();
        }

        public void InitializeWithSubTiles(WorldTile tile, AxialCoordinate position, List<WorldNode> connectedNodes)
        {
            ConnectedNodes = connectedNodes;
            connectedNodes.ForEach(t => t.isSubTile = true);
            Initialize(tile, position);
            CreateTile();
        }
        private void CreateTile()
        {
            if (tile != null) Destroy(tile);
            int rotationSteps = UnityEngine.Random.Range(0, 6);
            Quaternion rotation = Quaternion.Euler(0, rotationSteps * 60f, 0);
            tile = Instantiate(worldTile.prefab, transform.position, rotation, transform);
            CalculateProduction();

            maxCapacities = ConnectedNodes
            .SelectMany(n => n.InputBonuses)
            .Concat(worldTile.inputBonuses)
            .GroupBy(bonus => bonus.input)
            .ToDictionary(group => group.Key.type, group => group.Sum(bonus => bonus.maxCapacity));
        }

        public List<INode> Neighbors(Dictionary<AxialCoordinate, INode> allNodes)
        {
            List<INode> neighbors = new();

            foreach (var dir in HexMetrics.Directions)
            {
                AxialCoordinate neighborPos = Position + dir;
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
            cumulatedBonus = 0f;

            foreach (ResourceBonus bonus in InputBonuses)
            {
                Dictionary<ResourceType, int> incomingResources = TransportManager.GetIncomingResourcesFor(incomingRoutes);
                if (incomingResources.TryGetValue(bonus.input.type, out int amount))
                {
                    float effectiveBonus = Mathf.Min(amount, bonus.maxCapacity) * (bonus.bonusMultiplier / 100f);
                    cumulatedBonus += effectiveBonus;
                }
            }
            Production *= 1 + cumulatedBonus;
        }

        public float GetAvailableProduction()
        {
            if (isSubTile)
            {
                return 0;
            }

            float totalOutput = Production;
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
}