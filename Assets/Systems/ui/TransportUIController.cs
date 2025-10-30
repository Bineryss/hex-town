using System;
using Systems.Transport;
using UnityEngine;

namespace Systems.UI
{
    public class TransportUIController : MonoBehaviour, IUIController
    {
        [SerializeField] private TransportController transportController;
        private TransportRoutePanel transportRoutePanel;
        public IUIModeSegment UIModeSegment => transportRoutePanel;

        public void Initialize()
        {
            transportController.Initialize();

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
                transportController.PreviewRoute(origin, node);
            }

            if (isClick && origin != null && destination != null)
            {
                transportRoutePanel.ConfirmRouteCreation();
            }
        }

        private void HandleRouteDeletion(Guid guid)
        {
            transportController.TryDeleteRoute(guid);
        }
        private void HandleRouteCreation(WorldNode origin, WorldNode destination)
        {
            transportController.TryCreateRoute(origin, destination);
        }
        private void HandleRouteSelection(TransportRoute route)
        {
            transportController.SelectRoute(route.Id);
        }

        public void Exit()
        {
            transportRoutePanel.ExitMode();
        }

        public void Activate()
        {
            transportRoutePanel.EnterMode();
        }
    }
}