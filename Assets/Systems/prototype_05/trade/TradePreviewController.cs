using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using Systems.Prototype_04;
using Systems.Prototype_05.Player;
using Systems.Prototype_05.Transport;
using Systems.Prototype_05.UI;
using UnityEngine;
using UnityEngine.Pool;

namespace Systems.Prototype_05.Building
{
    public class TradePreviewController : SerializedMonoBehaviour, IPreviewController
    {
        [SerializeField] private HexGridGenerator generator;
        [SerializeField] private SmoothLineRenderer routePreview;
        [SerializeField] private PathfindingController pathfindingController;
        [SerializeField] private float offsetHeight = 0.6f;
        [SerializeField] private TransportController transportController;
        [SerializeField] private GameObject tradeRouteOriginIndicator;

        private ObjectPool<SmoothLineRenderer> lineVis;
        private HashSet<SmoothLineRenderer> activeLines = new();
        private HashSet<GameObject> activeIndicators = new();
        private TradeRouteDS tradeRoutes = TradeRouteDS.Instance;
        private Transform cameraTransform;


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
            cameraTransform = Camera.main.transform;
            tradeRoutes.routes = new();
            this.mode = mode;
            lineVis = new(
                () => Instantiate(routePreview, transform),
                (line) => line.ShowLine(),
                (line) => line.HideLine(),
                (line) => Destroy(line),
                true, 10, 50
            );

            EventBus<TileSelectionChanged>.Event += HandleMouseInteraction;
            // EventBus<TradeRouteVisualizationRequested>.Event += HandleVisualization;
        }

        void LateUpdate()
        {
            foreach (GameObject gO in activeIndicators)
            {
                gO.transform.rotation = cameraTransform.rotation;
            }
        }

        private List<ScorePreview> HandleVisualization(TradeRouteVisualizationRequested data)
        {
            foreach (GameObject gO in activeIndicators)
            {
                Destroy(gO);
            }
            activeIndicators.Clear();
            foreach (SmoothLineRenderer line in activeLines)
            {
                lineVis.Release(line);
            }
            activeLines.Clear();
            List<ScorePreview> tradeInformations = new();
            foreach (Guid id in data.Incoming)
            {
                if (tradeRoutes.routes.TryGetValue(id, out TransportRoute route))
                {
                    SmoothLineRenderer line = lineVis.Get();
                    line.RenderLine(route.path.ConvertAll(p => generator.layout.AxialToWorld(p)).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
                    line.ChangeColor(Color.white);
                    activeLines.Add(line);

                    Vector3 vector3 = generator.layout.AxialToWorld(route.origin.Position);
                    GameObject originIndiactor = Instantiate(tradeRouteOriginIndicator, new(vector3.x, offsetHeight + 0.01f, vector3.z), Quaternion.identity, transform);
                    activeIndicators.Add(originIndiactor);

                    tradeInformations.Add(new ScorePreview()
                    {
                        Position = route.origin.Position,
                        Icon = route.resourceType.ToString(),
                        Score = Mathf.FloorToInt(route.quantity)
                    });
                }
            }

            foreach (Guid id in data.Outgoing)
            {
                if (tradeRoutes.routes.TryGetValue(id, out TransportRoute route))
                {
                    SmoothLineRenderer line = lineVis.Get();
                    line.RenderLine(route.path.ConvertAll(p => generator.layout.AxialToWorld(p)).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
                    line.ChangeColor(Color.white);
                    activeLines.Add(line);
                    activeIndicators.Add(Instantiate(tradeRouteOriginIndicator, generator.layout.AxialToWorld(route.destination.Position), Quaternion.identity, transform));

                    Vector3 vector3 = generator.layout.AxialToWorld(route.origin.Position);
                    GameObject originIndiactor = Instantiate(tradeRouteOriginIndicator, new(vector3.x, offsetHeight + 0.01f, vector3.z), Quaternion.identity, transform);
                    activeIndicators.Add(originIndiactor);

                    tradeInformations.Add(new ScorePreview()
                    {
                        Position = route.destination.Position,
                        Icon = route.resourceType.ToString(),
                        Score = Mathf.FloorToInt(route.quantity)
                    });
                }
            }
            Debug.Log(tradeInformations.Count);
            return tradeInformations;
        }

        private void HandleMouseInteraction(TileSelectionChanged data)
        {
            WorldNode node = data.node;
            bool clicked = data.clicked;

            if (node == null) return;

            List<ScorePreview> preview = HandleVisualization(new TradeRouteVisualizationRequested()
            {
                Incoming = node.incomingRoutes,
                Outgoing = node.outgoingRoutes
            });

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
                    }.Concat(preview).ToList()
                });
            }

            if (origin == null && clicked)
            {
                // EventBus<TradeRouteVisualizationRequested>.Raise(new TradeRouteVisualizationRequested()
                // {
                //     Outgoing = data.node.outgoingRoutes,
                //     Incoming = data.node.incomingRoutes
                // });

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
                    }.Concat(preview).ToList()
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

    public struct TradeRouteVisualizationRequested : IEvent
    {
        public List<Guid> Incoming;
        public List<Guid> Outgoing;
    }
}