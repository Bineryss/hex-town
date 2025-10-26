using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
        [SerializeField] private TransportController transportController;
        [SerializeField] private TransportUIController transportUIController;

        [Header("Debug Info")]
        [SerializeField, ReadOnly] private WorldNode selectedNode;
        [SerializeField] private UIState currentState = UIState.INSPECTING;

        private VisualElement Root => uiDocument.rootVisualElement;
        private InspectPanel inspectPanel;
        private BuildingSelectionPanel buildingListPanel;
        private ModeSelectionPanel modeSelectionPanel;
        private WorldTile selectedBuilding;
        private readonly Dictionary<UIState, IUIModeSegment> uiModes = new();
        private readonly Dictionary<UIState, Action<WorldNode, bool>> uiModeActions = new();

        void Start()
        {
            if (uiDocument == null && TryGetComponent(out UIDocument uiDoc))
            {
                uiDocument = uiDoc;
            }

            uiModeActions[UIState.INSPECTING] = InspectAction;
            uiModeActions[UIState.BUILDING] = BuildAction;
            uiModeActions[UIState.MANAGING_TRANSPORT] = transportUIController.HandleMouseInteraction;
        }

        void OnEnable()
        {
            transportUIController.Initialize();

            inspectPanel = new();
            buildingListPanel = new(buildings);
            modeSelectionPanel = new(modes);

            playerGridSelector.OnChange += HandleMouseChange;
            buildingListPanel.OnBuildingSelected += HandleBuildingSelection;
            modeSelectionPanel.OnModeSelected += HandleModeSelection;

            uiModes[UIState.INSPECTING] = inspectPanel;
            uiModes[UIState.BUILDING] = buildingListPanel;
            uiModes[UIState.MANAGING_TRANSPORT] = transportUIController.UIModeSegment;

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
            node.Select();
            uiModeActions[currentState](node, isClick);
            selectedNode = node;
        }

        private void BuildAction(WorldNode node, bool isClick)
        {
            if (!isClick) return;
            if (selectedBuilding == null) return;
            if (!node.worldTile.isBuildable) return;

            transportController.Manager.RemoveAllRoutesForTile(node);
            node.name = $"{selectedBuilding.resourceType}-{node.Position}";
            node.Initialize(selectedBuilding, node.Position);
        }
        private void InspectAction(WorldNode node, bool isClick)
        {
            if (!isClick) return;
            if (node == null)
            {
                inspectPanel.UpdateTileInfo(new TileInformation()
                {
                    TileName = "NAN"
                });
            }
            else
            {
                Dictionary<ResourceType, int> incomingResources = transportController.Manager.GetIncomingResourcesFor(node.incomingRoutes);
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