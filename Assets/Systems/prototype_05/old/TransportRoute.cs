using System;
using System.Collections.Generic;
using Systems.Prototype_04.Building;
using Systems.Prototype_04.Grid;
using UnityEngine;

namespace Systems.Prototype_05.Transport
{

    [Serializable]
    public class TransportRoute
    {
        public static readonly float MAX_DISTANCE = 10f;
        public static readonly float MIN_DISTANCE = 3f;
        public static readonly float DECAY_CONSTANT = 3.5f;


        public Guid Id { get; private set; }
        public WorldNode origin;
        public WorldNode destination;
        public ResourceType resourceType;
        public float quantity;
        public List<HexCoordinate> path = new();
        private readonly float efficiencyMultiplier;

        public TransportRoute(WorldNode origin, WorldNode destination, ResourceType resourceType, float quantity, List<HexCoordinate> path)
        {
            Id = Guid.NewGuid();
            this.origin = origin;
            this.destination = destination;
            this.resourceType = resourceType;
            this.quantity = quantity;
            this.path = path;
            efficiencyMultiplier = CalculateEfficiency(path.Count);
        }

        private float CalculateEfficiency(int distance)
        {
            if (distance <= MIN_DISTANCE) return 1.0f; // 100% delivery
            if (distance >= MAX_DISTANCE) return 0f; // Nothing arrives

            float normalizedDistance = (distance - MIN_DISTANCE) / (MAX_DISTANCE - MIN_DISTANCE); // Normalize to 0-1
            float k = DECAY_CONSTANT; // Decay constant (adjust for desired curve)
            return Mathf.Exp(-k * normalizedDistance);
        }

        public int GetDeliveredAmount()
        {
            return Mathf.FloorToInt(quantity * efficiencyMultiplier);
        }
    }
}