using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using Systems.Transport;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.UI
{

    public class GameUIController : SerializedMonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private PlayerGridSelector playerGridSelector;
        [SerializeField] private List<WorldTile> buildings;
        [SerializeField] private List<ModeElement> modes = new();

        [Header("Transport UI")]
        [SerializeField] private SmoothLineRenderer smoothLineRendererPrefab;
        [SerializeField] private HexGrid grid;
        [SerializeField] private TransportManager transportManager;
        [SerializeField] private PathfindingController pathfindingController;
        [SerializeField] private float offsetHeight = 0.6f;
        private SmoothLineRenderer transportRoutePreview;

        [Header("Debug Info")]
        [SerializeField, ReadOnly] private WorldNode selectedNode;
        [SerializeField] private UIState currentState = UIState.INSPECTING;

        private VisualElement Root => uiDocument.rootVisualElement;
        private InspectPanel inspectPanel;
        private BuildingSelectionPanel buildingListPanel;
        private TransportRoutePanel transportRoutePanel;
        private ModeSelectionPanel modeSelectionPanel;
        private WorldTile selectedBuilding;
        private readonly Dictionary<UIState, IUIModeSegment> uiModes = new();
        private readonly Dictionary<Guid, SmoothLineRenderer> transportVisuailizer = new();
        private SmoothLineRenderer selectedRoute;

        void Start()
        {
            if (uiDocument == null && TryGetComponent(out UIDocument uiDoc))
            {
                uiDocument = uiDoc;
            }
            transportRoutePreview = Instantiate(smoothLineRendererPrefab, transform);
        }

        void Update()
        {
            transportRoutePanel.UpdateRoutes(transportManager.GetAllRoutes());
        }

        void OnEnable()
        {
            inspectPanel = new();
            buildingListPanel = new(buildings);
            modeSelectionPanel = new(modes);
            transportRoutePanel = new(transportManager.GetAllRoutes());

            // playerGridSelector.OnNodeSelected += HandleSelection;
            playerGridSelector.OnChange += HandleMouseChange;
            buildingListPanel.OnBuildingSelected += HandleBuildingSelection;
            transportRoutePanel.OnRouteDeleted += HandleRouteDeletion;
            transportRoutePanel.OnCreateRouteConfirmed += HandleRouteCreation;
            transportRoutePanel.OnRouteSelected += HandleRouteSelection;
            modeSelectionPanel.OnModeSelected += HandleModeSelection;

            uiModes[UIState.INSPECTING] = inspectPanel;
            uiModes[UIState.BUILDING] = buildingListPanel;
            uiModes[UIState.MANAGING_TRANSPORT] = transportRoutePanel;

            BuildUI();
        }

        private void HandleMouseChange(WorldNode node, bool isClick)
        {
            if (node == null) return;
            if (currentState == UIState.EXPLORING) return;
            if (selectedNode != null)
            {
                selectedNode.Deselect();
            }
            selectedNode = node;
            node.Select();

            if (currentState == UIState.INSPECTING)
            {
                if (!isClick) return;
                if (selectedNode == null)
                {
                    inspectPanel.UpdateTileInfo(new TileInformation()
                    {
                        TileName = "NAN"
                    });
                }
                else
                {
                    Dictionary<ResourceType, int> incomingResources = transportManager.GetIncomingResourcesFor(node.incomingRoutes);
                    List<BonusInformation> bonusInfos = new();

                    foreach (ResourceBonus bonus in node.worldTile.inputBonuses)
                    {
                        incomingResources.TryGetValue(bonus.input, out int amount);
                        bonusInfos.Add(new BonusInformation
                        {
                            ResourceType = bonus.input,
                            BonusMultiplier = bonus.bonusMultiplier,
                            MaxCapacity = bonus.maxCapacity,
                            CurrentInputAmount = amount
                        });
                    }


                    inspectPanel.UpdateTileInfo(new TileInformation
                    {
                        TileName = node.worldTile.name,
                        ProductionType = node.ResourceType,
                        ProductionRate = node.Production,
                        AvailableResources = node.GetAvailableProduction(),
                        BonusInformations = bonusInfos,
                        CumulatedBonus = node.worldTile.resourceAmount > 0 ? Mathf.FloorToInt(100 * (node.Production / node.worldTile.resourceAmount)) : 0f
                    });
                }
            }
            else if (currentState == UIState.BUILDING)
            {
                if (!isClick) return;
                if (selectedBuilding == null) return;
                if (!node.worldTile.isBuildable) return;

                transportManager.RemoveAllRoutesForTile(node);
                node.name = $"{selectedBuilding.resourceType}-{node.Position}";
                node.Initialize(selectedBuilding, node.Position);
            }
            else if (currentState == UIState.MANAGING_TRANSPORT)
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
                    HandleRoutePreview(origin, node);
                }

                if (isClick && origin != null && destination != null)
                {
                    transportRoutePanel.ConfirmRouteCreation();
                    transportRoutePreview.HideLine();
                }
            }

        }

        private void HandleRouteDeletion(Guid guid)
        {
            transportManager.RemoveRoute(guid);
            if (!transportVisuailizer.TryGetValue(guid, out SmoothLineRenderer line)) return;
            if (line.Equals(selectedRoute))
            {
                selectedRoute = null;
            }
            transportVisuailizer.Remove(guid);
            Destroy(line.gameObject);
        }
        private void HandleRouteCreation(WorldNode origin, WorldNode destination)
        {
            if (origin == null || destination == null) return;
            if (selectedRoute != null)
            {
                selectedRoute.HideLine();
            }


            TransportRoute route = transportManager.CreateRoute(origin, destination);
            if (route == null) return;
            SmoothLineRenderer newLine = Instantiate(smoothLineRendererPrefab, transform);
            newLine.RenderLine(route.path.ConvertAll(p => grid.Grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            transportVisuailizer[route.Id] = newLine;
            newLine.ShowLine();
            selectedRoute = newLine;
        }

        private void HandleRoutePreview(WorldNode origin, WorldNode destination)
        {
            if (selectedRoute != null)
            {
                selectedRoute.HideLine();
            }

            List<HexCoordinate> path = pathfindingController.FindPath(origin, destination);
            transportRoutePreview.RenderLine(path.ConvertAll(p => grid.Grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            transportRoutePreview.ShowLine();
        }
        private void HandleRouteSelection(TransportRoute route)
        {
            transportRoutePreview.HideLine();
            if (!transportVisuailizer.TryGetValue(route.Id, out SmoothLineRenderer line)) return;
            if (selectedRoute != null)
            {
                selectedRoute.HideLine();
            }
            if (line.Equals(selectedRoute)) return;

            line.ShowLine();

            selectedRoute = line;
        }

        private void HandleModeSelection(UIState state)
        {
            if (state == currentState) return;

            if (selectedNode != null)
            {
                selectedNode.Deselect();
                selectedNode = null;
            }
            foreach (IUIModeSegment mode in uiModes.Values)
            {
                mode.ExitMode();
            }


            if (uiModes.TryGetValue(state, out var selectedMode))
            {
                selectedMode.EnterMode();
            }
            currentState = state;
        }

        private void HandleBuildingSelection(WorldTile building)
        {
            if (building == null) return;
            selectedBuilding = building;
            selectedNode = null;
        }
        private void HandleSelection(WorldNode node)
        {
            if (node == null) return;
            if (currentState == UIState.EXPLORING) return;

            if (selectedNode != null)
            {
                selectedNode.Deselect();
            }
            if (selectedNode != null && node.Position.Equals(selectedNode.Position))
            {
                selectedNode = null;
                return;
            }


            selectedNode = node;
            node.Select();

            if (currentState == UIState.INSPECTING)
            {
                if (selectedNode == null)
                {
                    inspectPanel.UpdateTileInfo(new TileInformation()
                    {
                        TileName = "NAN"
                    });
                }
                else
                {
                    Dictionary<ResourceType, int> incomingResources = transportManager.GetIncomingResourcesFor(node.incomingRoutes);
                    List<BonusInformation> bonusInfos = new();

                    foreach (ResourceBonus bonus in node.worldTile.inputBonuses)
                    {
                        incomingResources.TryGetValue(bonus.input, out int amount);
                        bonusInfos.Add(new BonusInformation
                        {
                            ResourceType = bonus.input,
                            BonusMultiplier = bonus.bonusMultiplier,
                            MaxCapacity = bonus.maxCapacity,
                            CurrentInputAmount = amount
                        });
                    }


                    inspectPanel.UpdateTileInfo(new TileInformation
                    {
                        TileName = node.worldTile.name,
                        ProductionType = node.ResourceType,
                        ProductionRate = node.Production,
                        AvailableResources = node.GetAvailableProduction(),
                        BonusInformations = bonusInfos,
                        CumulatedBonus = Mathf.FloorToInt(100 * (node.Production / node.worldTile.resourceAmount))
                    });
                }
            }
            else if (currentState == UIState.BUILDING)
            {
                if (selectedBuilding == null) return;
                if (!node.worldTile.isBuildable) return;

                transportManager.RemoveAllRoutesForTile(node);
                node.name = $"{selectedBuilding.resourceType}-{node.Position}";
                node.Initialize(selectedBuilding, node.Position);
            }
            else if (currentState == UIState.MANAGING_TRANSPORT)
            {
                WorldNode origin = transportRoutePanel.SelectedOrigin;

                if (origin == null)
                {
                    transportRoutePanel.SetOriginNode(node);
                    return;
                }

                if (node.Position.Equals(origin))
                {
                    transportRoutePanel.SetOriginNode(null);
                }
                else
                {
                    transportRoutePanel.SetDestinationNode(node);
                }
            }
        }

        private void BuildUI()
        {
            Root.Clear();

            Label label = new("Game UI Initialized");
            Root.Add(label);


            VisualElement menuContainer = new();
            menuContainer.style.maxWidth = 300;
            menuContainer.style.flexGrow = 1;
            menuContainer.style.flexDirection = FlexDirection.Column;
            foreach (IUIModeSegment segment in uiModes.Values)
            {
                segment.ExitMode();
                menuContainer.Add(segment as VisualElement);
            }

            Root.Add(menuContainer);

            Root.Add(modeSelectionPanel);
        }
    }

}