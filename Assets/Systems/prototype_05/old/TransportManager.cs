using System;
using System.Collections.Generic;
using Systems.Core;
using Systems.Prototype_04.Building;

namespace Systems.Prototype_05.Transport
{
    public class TransportManager
    {
        private readonly PathfindingController pathfindingController;
        private readonly Dictionary<Guid, TransportRoute> transportRoutes = new();

        public TransportManager(PathfindingController pathfindingController, Dictionary<Guid, TransportRoute> transportRoutes)
        {
            this.pathfindingController = pathfindingController;
            this.transportRoutes = transportRoutes;
        }

        public bool CanCreateRoute(WorldNode origin, WorldNode destination, out string errorMessage, out List<AxialCoordinate> path)
        {
            errorMessage = "";
            bool canCreate = true;

            if (origin == null || destination == null)
            {
                errorMessage = "Origin or destination is null.";
                canCreate = false;
            }
            if (origin == destination)
            {
                errorMessage = "Origin and destination cannot be the same.";
                canCreate = false;
            }
            if (origin.ResourceType.Equals(ResourceType.NONE))
            {
                errorMessage = "Origin does not produce any resources.";
                canCreate = false;
            }
            if (destination.isSubTile)
            {
                errorMessage = "Destination cannot be a sub-tile.";
                canCreate = false;
            }
            if (!destination.AcceptedInputResources.Contains(origin.ResourceType))
            {
                errorMessage = $"Destination does not accept {origin.ResourceType}.";
                canCreate = false;
            }
            if (origin.GetAvailableProduction() <= 0)
            {
                errorMessage = "Origin has no available production to transport.";
                canCreate = false;
            }

            path = pathfindingController.FindPath(origin, destination);
            if (path.Count == 0)
            {
                errorMessage = "No valid path found between origin and destination.";
                canCreate = false;
            }

            return canCreate;
        }

        public TransportRoute CreateRoute(WorldNode origin, WorldNode destination)
        {
            if (!CanCreateRoute(origin, destination, out string errorMessage, out List<AxialCoordinate> path))
            {
                return null;
            }

            destination.MaxIncomingCapacity.TryGetValue(origin.ResourceType, out int maxCapacity);

            TransportRoute newRoute = new(origin, destination, origin.ResourceType, Math.Min(maxCapacity, origin.GetAvailableProduction()), path);
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


