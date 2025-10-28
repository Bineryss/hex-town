using System;
using System.Collections.Generic;
using Systems.Grid;
using UnityEngine;

namespace Systems.Transport
{
    public class TransportController : MonoBehaviour
    {
        public static TransportController Instance { get; private set; }
        public TransportManager Manager => transportManager;

        [SerializeField] private HexGrid grid;
        [SerializeField] private SmoothLineRenderer smoothLineRendererPrefab;
        [SerializeField] private PathfindingController pathfindingController;
        [SerializeField] private float offsetHeight = 0.6f;
        private TransportManager transportManager;

        private Dictionary<Guid, SmoothLineRenderer> transportRouteVisualizers = new();
        private SmoothLineRenderer previewedRoute;
        private Guid? selectedRouteId = null;

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
            previewedRoute = Instantiate(smoothLineRendererPrefab, transform);
        }

        public TransportRoute CreateRoute(WorldNode origin, WorldNode destination)
        {
            TransportRoute newRoute = transportManager.CreateRoute(origin, destination);
            return newRoute;
        }

        public bool TryDeleteRoute(Guid routeId)
        {
            transportManager.RemoveRoute(routeId);
            if (transportRouteVisualizers.TryGetValue(routeId, out SmoothLineRenderer visualizer))
            {
                Destroy(visualizer.gameObject);
            }
            transportRouteVisualizers.Remove(routeId);
            if (selectedRouteId == routeId)
            {
                selectedRouteId = null;
            }
            return true;
        }

        public bool TryCreateRoute(WorldNode origin, WorldNode destination)
        {
            previewedRoute.HideLine();
            if (selectedRouteId != null)
            {
                transportRouteVisualizers[selectedRouteId.Value].HideLine();
            }

            TransportRoute route = transportManager.CreateRoute(origin, destination);
            if (route == null) return false;

            SmoothLineRenderer lineRenderer = Instantiate(smoothLineRendererPrefab);
            transportRouteVisualizers[route.Id] = lineRenderer;
            lineRenderer.RenderLine(route.path.ConvertAll(p => grid.Grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            lineRenderer.ShowLine();
            selectedRouteId = route.Id;

            return true;
        }

        public void PreviewRoute(WorldNode origin, WorldNode destination)
        {
            if (selectedRouteId != null)
            {
                transportRouteVisualizers[selectedRouteId.Value].HideLine();
                selectedRouteId = null;
            }

            if (!transportManager.CanCreateRoute(origin, destination, out string errorMessage, out List<HexCoordinate> path))
            {
                previewedRoute.ChangeColor(Color.red);
                Debug.Log($"Cannot commit route: {errorMessage}");
            }
            else
            {
                previewedRoute.ChangeColor(Color.green);
            }

            previewedRoute.RenderLine(path.ConvertAll(p => grid.Grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            previewedRoute.ShowLine();
        }

        public void SelectRoute(Guid routeId)
        {
            previewedRoute.HideLine();
            if (selectedRouteId != null)
            {
                transportRouteVisualizers[selectedRouteId.Value].HideLine();
            }

            if (transportRouteVisualizers.TryGetValue(routeId, out SmoothLineRenderer visualizer))
            {
                visualizer.ShowLine();
                selectedRouteId = routeId;
            }
        }
    }
}