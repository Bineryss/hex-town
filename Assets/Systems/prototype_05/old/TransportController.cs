using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Prototype_05.Transport
{
    public class TransportController : MonoBehaviour
    {
        public static TransportController Instance { get; private set; }
        public TransportManager Manager => transportManager;

        [SerializeField] private PathfindingController pathfindingController;
        private TransportManager transportManager;


        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Initialize()
        {
            transportManager = new TransportManager(pathfindingController, new Dictionary<Guid, TransportRoute>());
        }

        public TransportRoute CreateRoute(WorldNode origin, WorldNode destination)
        {
            TransportRoute newRoute = transportManager.CreateRoute(origin, destination);
            return newRoute;
        }

        public bool TryDeleteRoute(Guid routeId)
        {
            transportManager.RemoveRoute(routeId);
            return true;
        }

        public bool TryCreateRoute(WorldNode origin, WorldNode destination, out TransportRoute route)
        {
            route = transportManager.CreateRoute(origin, destination);
            if (route == null) return false;

            return true;
        }
    }
}