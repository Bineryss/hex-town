using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using UnityEngine;

namespace Systems.Transport
{
    public class TransportManager : SerializedMonoBehaviour
    {
        #region Singleton
        public static TransportManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        [SerializeField] private PathfindingController pathfindingController;
        [OdinSerialize] private Dictionary<Guid, TransportRoute> transportRoutes = new();

        public bool CanCreateRoute(WorldNode origin, WorldNode destination, out string errorMessage, out List<HexCoordinate> path)
        {
            errorMessage = "";
            path = new List<HexCoordinate>();

            if (origin == null || destination == null)
            {
                errorMessage = "Origin or destination is null.";
                return false;
            }
            if (origin == destination)
            {
                errorMessage = "Origin and destination cannot be the same.";
                return false;
            }
            if (origin.ResourceType.Equals(ResourceType.NONE))
            {
                errorMessage = "Origin does not produce any resources.";
                return false;
            }
            if (!destination.AcceptedInputResources.Contains(origin.ResourceType))
            {
                errorMessage = $"Destination does not accept {origin.ResourceType}.";
                return false;
            }
            if (origin.GetAvailableProduction() <= 0)
            {
                errorMessage = "Origin has no available production to transport.";
                return false;
            }

            path = pathfindingController.FindPath(origin, destination);
            if (path.Count == 0)
            {
                errorMessage = "No valid path found between origin and destination.";
                return false;
            }

            return true;
        }

        public TransportRoute CreateRoute(WorldNode origin, WorldNode destination)
        {
            if (!CanCreateRoute(origin, destination, out string errorMessage, out List<HexCoordinate> path))
            {
                Debug.Log($"Cannot create route: {errorMessage}");
                return null;
            }

            TransportRoute newRoute = new(origin, destination, origin.ResourceType, origin.GetAvailableProduction(), path);
            transportRoutes[newRoute.Id] = newRoute;
            origin.AddOutgoingRoute(newRoute.Id);
            destination.AddIncomingRoute(newRoute.Id);
            return newRoute;
        }

        public void RemoveRoute(Guid routeId)
        {
            if (!transportRoutes.TryGetValue(routeId, out TransportRoute route)) return;

            route.origin.RemoveRoute(routeId);
            route.destination.RemoveRoute(routeId);
            transportRoutes.Remove(routeId);
        }

        public void RemoveAllRoutesForTile(WorldNode tile)
        {
            List<Guid> routesToRemove = new();

            routesToRemove.AddRange(tile.outgoingRoutes);
            routesToRemove.AddRange(tile.incomingRoutes);

            foreach (Guid routeId in routesToRemove)
            {
                RemoveRoute(routeId);
            }
        }

        public TransportRoute GetRoute(Guid routeId)
        {
            transportRoutes.TryGetValue(routeId, out TransportRoute route);
            return route;
        }

        public Dictionary<ResourceType, int> GetIncomingResourcesFor(List<Guid> routeIds)
        {
            Dictionary<ResourceType, int> resources = new();
            foreach (Guid routeId in routeIds)
            {
                if (transportRoutes.TryGetValue(routeId, out TransportRoute route))
                {
                    if (resources.TryGetValue(route.resourceType, out int currentAmount))
                    {
                        resources[route.resourceType] = currentAmount + route.GetDeliveredAmount();
                    }
                    else
                    {
                        resources[route.resourceType] = route.GetDeliveredAmount();
                    }
                }
            }
            return resources;
        }

        public List<TransportRoute> GetAllRoutes()
        {
            return new List<TransportRoute>(transportRoutes.Values);
        }
    }
}


