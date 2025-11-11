using System;
using System.Collections.Generic;
using Systems.Core;
using Systems.Prototype_04;
using Systems.Prototype_05.Player;
using Systems.Prototype_05.Transport;
using Systems.Prototype_05.UI;
using UnityEngine;

namespace Systems.Prototype_05.Building
{
    public class TradePreviewController : MonoBehaviour, IPreviewController
    {
        [SerializeField] private HexGridGenerator generator;
        [SerializeField] private SmoothLineRenderer routePreview;
        [SerializeField] private PathfindingController pathfindingController;
        [SerializeField] private float offsetHeight = 0.6f;
        [SerializeField] private TransportController transportController;

        public Guid Mode => mode;

        public bool Active //TODO extract into exit and enter methods
        {
            get => active; set
            {
                active = value;
                routePreview.HideLine();
                origin = null;
                destination = null;
                EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested());
            }
        }
        private bool active;
        private Guid mode;
        private WorldNode origin;
        private WorldNode destination;

        public void Initialize(Guid mode)
        {
            this.mode = mode;

            EventBus<TileSelectionChanged>.Event += HandleMouseInteraction;
        }

        private void HandleMouseInteraction(TileSelectionChanged data)
        {
            WorldNode node = data.node;
            bool clicked = data.clicked;

            if (active)
            {
                EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested()
                {
                    Tooltips = new List<ScorePreview>()
                    {
                        new()
                        {
                            Position = node.Position,
                            Icon = node.ResourceType.ToString(),
                            Score = Mathf.FloorToInt(node.Production)
                        }
                    }
                });
            }

            if (origin == null && clicked)
            {
                EventBus<PreviewActivationRequested>.Raise(new PreviewActivationRequested()
                {
                    Mode = mode
                });
                origin = node;
                return;
            }

            if (origin == null) return;
            if (!active)
            {
                origin = null;
                return;
            }

            EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested()
            {
                Tooltips = new List<ScorePreview>()
                    {
                        new()
                        {
                            Position = node.Position,
                            Icon = origin.ResourceType.ToString(),
                            Score = Mathf.FloorToInt(origin.Production)
                        }
                    }
            });
            if (node.Position.Equals(origin.Position) && clicked)
            {
                origin = null;
                EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested());
            }
            else
            {
                destination = node;
                PreviewRoute(origin, node);
            }

            if (clicked && origin != null && destination != null)
            {
                HandleRouteCreation(origin, destination);
            }
        }

        private void HandleRouteCreation(WorldNode origin, WorldNode destination)
        {
            if (!transportController.TryCreateRoute(origin, destination, out TransportRoute route)) return;

            routePreview.RenderLine(route.path.ConvertAll(p => generator.layout.AxialToWorld(p)).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            routePreview.ShowLine();
            origin = null;
            destination = null;
            EventBus<PreviewDeactivationRequested>.Raise(new PreviewDeactivationRequested()
            {
                Mode = mode
            });
        }

        private void PreviewRoute(WorldNode origin, WorldNode destination)
        {

            if (!transportController.Manager.CanCreateRoute(origin, destination, out string errorMessage, out List<AxialCoordinate> path))
            {
                routePreview.ChangeColor(Color.red);
                Debug.Log($"Cannot commit route: {errorMessage}");
            }
            else
            {
                routePreview.ChangeColor(Color.green);
            }

            routePreview.RenderLine(path.ConvertAll(p => generator.layout.AxialToWorld(p)).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            routePreview.ShowLine();
        }

    }
}