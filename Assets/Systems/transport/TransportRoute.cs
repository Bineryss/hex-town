using System;
using System.Collections.Generic;
using Systems.Grid;
using UnityEngine;

namespace Systems.Transport
{

    [System.Serializable]
    public class TransportRoute
    {
        public static readonly float MAX_DISTANCE = 10f;
        public static readonly float MIN_DISTANCE = 3f;
        public static readonly float DECAY_CONSTANT = 3.5f;


        public Guid Id { get; private set; }
        public WorldNode origin;
        public WorldNode destination;
        public ResourceType resourceType;
        public int quantity;
        public List<HexCoordinate> path = new();
        private float efficiencyMultiplier = 0f;

        public TransportRoute(WorldNode origin, WorldNode destination, ResourceType resourceType, int quantity, List<HexCoordinate> path)
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
            if (distance <= TransportRoute.MIN_DISTANCE) return 1.0f; // 100% delivery
            if (distance >= TransportRoute.MAX_DISTANCE) return 0f; // Nothing arrives

            float normalizedDistance = (distance - TransportRoute.MIN_DISTANCE) / (TransportRoute.MAX_DISTANCE - TransportRoute.MIN_DISTANCE); // Normalize to 0-1
            float k = TransportRoute.DECAY_CONSTANT; // Decay constant (adjust for desired curve)
            return Mathf.Exp(-k * normalizedDistance);
        }

        public int GetDeliveredAmount()
        {
            return Mathf.FloorToInt(quantity * efficiencyMultiplier);
        }
    }
}