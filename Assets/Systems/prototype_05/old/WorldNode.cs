using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using Systems.Prototype_04;
using Systems.Prototype_04.Building;
using Systems.Prototype_04.Grid;
using Systems.Prototype_05.Score;
using Systems.Prototype_05.Transport;
using UnityEngine;

namespace Systems.Prototype_05
{
    public class WorldNode : SerializedMonoBehaviour, INode
    {
        private TransportManager TransportManager => TransportController.Instance.Manager;
        public WorldTile worldTile;

        [Header("Debug Info")]
        [ShowInInspector, ReadOnly] public HexCoordinate Position { get; set; }
        [ShowInInspector, ReadOnly]
        public bool IsWalkable
        {
            get
            {
                if (worldTile == null) return false;
                return worldTile.isWalkable;
            }
        }
        [ShowInInspector, ReadOnly]
        public int MovementCost
        {
            get
            {
                if (worldTile == null) return 0;
                return worldTile.movementCost;
            }
        }

        [Header("Resource Info")]
        [ShowInInspector, ReadOnly]
        public ResourceType ResourceType
        {
            get
            {

                if (worldTile == null) return default;
                if (ConnectedNodes.Count == 0) return worldTile.resourceType.type;
                return ConnectedNodes[0].ResourceType;
            }
        }
        [ShowInInspector, ReadOnly]
        public Resource Resource
        {
            get
            {

                if (worldTile == null) return default;
                if (ConnectedNodes.Count == 0) return worldTile.resourceType;
                return ConnectedNodes[0].Resource;
            }
        }
        [ShowInInspector, ReadOnly] public float Production;
        public List<WorldTile> ConnectableTiles
        {
            get
            {
                if (worldTile == null) return new();
                return worldTile.connectableTiles;
            }
        }

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
        private int currentScore;

        private bool isPlaced;

        public void ConfirmPlacement()
        {
            isPlaced = true;
            EventBus<ScoreChanged>.Raise(new ScoreChanged()
            {
                Delta = currentScore
            });
        }
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
            Production = worldTile.resourceAmount + ConnectedNodes.Sum(n => n.worldTile.resourceAmount);
            outgoingRoutes.Clear();
            incomingRoutes.Clear();
            CalculateProduction();
        }

        public void InitializeWithSubTiles(WorldTile tile, HexCoordinate position, List<WorldNode> connectedNodes)
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
            if (Production != currentScore)
            {
                int delta = Mathf.FloorToInt(Production) - currentScore;
                currentScore += delta;
                EventBus<ScoreCalculated>.Raise(new ScoreCalculated()
                {
                    Score = currentScore,
                });
                if (isPlaced)
                {
                    EventBus<ScoreChanged>.Raise(new ScoreChanged()
                    {
                        Delta = delta
                    });
                }
            }
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

    public struct ScoreCalculated : IEvent
    {
        public int Score;
    }
}