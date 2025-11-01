using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Grid;
using Systems.Transport;
using UnityEngine;

namespace Systems.UI
{
    public class TransportUIController : MonoBehaviour, IUIController
    {
        private TransportRoutePanel transportRoutePanel;
        public IUIModeSegment UIModeSegment => transportRoutePanel;

        [SerializeField] private TransportController transportController;

        [Header("Transport Path Visualization")]
        [SerializeField] private HexGrid grid;
        [SerializeField] private SmoothLineRenderer smoothLineRendererPrefab;
        [SerializeField] private PathfindingController pathfindingController;
        [SerializeField] private float offsetHeight = 0.6f;

        private readonly Dictionary<Guid, SmoothLineRenderer> transportRouteVisualizers = new();
        private SmoothLineRenderer previewedRoute;
        private Guid? selectedRouteId = null;

        public void Initialize()
        {
            transportController.Initialize();
            previewedRoute = Instantiate(smoothLineRendererPrefab, transform);
            transportRoutePanel = new(transportController.Manager.GetAllRoutes());
            transportRoutePanel.OnRouteDeleted += HandleRouteDeletion;
            transportRoutePanel.OnCreateRouteConfirmed += HandleRouteCreation;
            transportRoutePanel.OnRouteSelected += HandleRouteSelection;
        }

        void Update()
        {
            transportRoutePanel.UpdateRoutes(transportController.Manager.GetAllRoutes());
        }

        public void HandleMouseInteraction(WorldNode node, WorldNode prevNode, bool isClick)
        {
            WorldNode origin = transportRoutePanel.SelectedOrigin;
            WorldNode destination = transportRoutePanel.SelectedDestination;

            if (origin == null && isClick)
            {
                transportRoutePanel.SetOriginNode(node);
                return;
            }

            if (origin == null) return;

            if (node.Position.Equals(origin) && isClick)
            {
                transportRoutePanel.SetOriginNode(null);
            }
            else
            {
                transportRoutePanel.SetDestinationNode(node);
                PreviewRoute(origin, node);
            }

            if (isClick && origin != null && destination != null)
            {
                transportRoutePanel.ConfirmRouteCreation();
            }
        }

        private void PreviewRoute(WorldNode origin, WorldNode destination)
        {
            if (selectedRouteId != null)
            {
                transportRouteVisualizers[selectedRouteId.Value].HideLine();
                selectedRouteId = null;
            }

            if (!transportController.Manager.CanCreateRoute(origin, destination, out string errorMessage, out List<HexCoordinate> path))
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

        private void HandleRouteDeletion(Guid id)
        {
            transportController.TryDeleteRoute(id);
            if (transportRouteVisualizers.TryGetValue(id, out SmoothLineRenderer visualizer))
            {
                Destroy(visualizer.gameObject);
            }
            transportRouteVisualizers.Remove(id);
            if (selectedRouteId == id)
            {
                selectedRouteId = null;
            }
        }
        private void HandleRouteCreation(WorldNode origin, WorldNode destination)
        {
            previewedRoute.HideLine();
            if (selectedRouteId != null)
            {
                transportRouteVisualizers[selectedRouteId.Value].HideLine();
            }

            if (!transportController.TryCreateRoute(origin, destination, out TransportRoute route)) return;

            SmoothLineRenderer lineRenderer = Instantiate(smoothLineRendererPrefab, transform);
            transportRouteVisualizers[route.Id] = lineRenderer;
            lineRenderer.RenderLine(route.path.ConvertAll(p => grid.Grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            lineRenderer.ShowLine();
            selectedRouteId = route.Id;

        }
        private void HandleRouteSelection(TransportRoute route)
        {
            previewedRoute.HideLine();
            if (selectedRouteId != null)
            {
                transportRouteVisualizers[selectedRouteId.Value].HideLine();
            }

            if (transportRouteVisualizers.TryGetValue(route.Id, out SmoothLineRenderer visualizer))
            {
                visualizer.ShowLine();
                selectedRouteId = route.Id;
            }
        }

        public void Exit()
        {
            transportRouteVisualizers.Values.ToList().ForEach(v => v.HideLine());
            previewedRoute.HideLine();
            selectedRouteId = null;
            transportRoutePanel.ExitMode();
        }

        public void Activate()
        {
            transportRoutePanel.EnterMode();
        }
    }
}