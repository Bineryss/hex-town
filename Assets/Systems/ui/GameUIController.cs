using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlayerGridSelector playerGridSelector;
    [SerializeField] private List<WorldTile> buildings;

    [Header("Debug Info")]
    [SerializeField, ReadOnly] private WorldNode selectedNodeA;
    [SerializeField, ReadOnly] private WorldNode selectedNodeB;
    [SerializeField] private UIState currentState = UIState.INSPECTING;

    private VisualElement Root => uiDocument.rootVisualElement;
    private InspectPanel inspectPanel;
    private BuildingSelectionPanel buildingListPanel;
    private WorldTile selectedBuilding;
    private readonly Dictionary<UIState, IUIModeSegment> uiModes = new();

    void Start()
    {
        if (uiDocument == null && TryGetComponent<UIDocument>(out var uiDoc))
        {
            uiDocument = uiDoc;
        }
    }

    void Update()
    {
        foreach (IUIModeSegment mode in uiModes.Values)
        {
            mode.ExitMode();
        }


        if (uiModes.TryGetValue(currentState, out var selectedMode))
        {
            selectedMode.EnterMode();
        }
    }

    void OnEnable()
    {
        inspectPanel = new InspectPanel();
        buildingListPanel = new BuildingSelectionPanel();

        BuildUI();
        playerGridSelector.OnNodeSelected += HandleSelection;
        buildingListPanel.OnBuildingSelected += HandleBuildingSelection;

        uiModes[UIState.INSPECTING] = inspectPanel;
        uiModes[UIState.BUILDING] = buildingListPanel;
    }

    private void HandleBuildingSelection(WorldTile building)
    {
        if (building == null) return;
        selectedBuilding = building;
        selectedNodeA = null;
        selectedNodeB = null;
    }
    private void HandleSelection(WorldNode node)
    {
        if (node == null) return;

        if (currentState == UIState.INSPECTING)
        {
            if (selectedNodeA != null)
            {
                selectedNodeA.Deselect();
            }
            selectedNodeA = node;
            selectedNodeA.Select();
            inspectPanel.UpdateTileInfo(new TileInformation
            {
                TileName = node.worldTile.name,
                ProductionType = node.ResourceType,
                ProductionRate = node.Production,
                AvailableResources = node.GetAvailableProduction(),
                AcceptedResources = node.AcceptedInputResources.ToArray()
            });
        }
        else if (currentState == UIState.BUILDING)
        {
            if (selectedNodeA != null)
            {
                selectedNodeA.Deselect();
            }
            if (selectedBuilding == null) return;
            if (!node.worldTile.isBuildable) return;
            selectedNodeA = node;

            selectedNodeA.name = $"{selectedBuilding.resourceType}-{selectedNodeA.Position}";
            selectedNodeA.Initialize(selectedBuilding, selectedNodeA.Position);
            selectedNodeA.Select();
            selectedNodeA = null;
        }
        else if (currentState == UIState.MANAGING_TRANSPORT)
        {
            if (selectedNodeA == null)
            {
                selectedNodeA = node;
                selectedNodeA.Select();
            }
            else if (selectedNodeB == null && selectedNodeA != null && !node.Position.Equals(selectedNodeA.Position))
            {
                selectedNodeB = node;
                selectedNodeB.Select();
            }
            else
            {
                if (selectedNodeA.Position.Equals(node.Position))
                {
                    selectedNodeA.Deselect();
                    selectedNodeA = null;
                }
                else if (selectedNodeB != null && selectedNodeB.Position.Equals(node.Position))
                {
                    selectedNodeB.Deselect();
                    selectedNodeB = null;
                }
            }
        }
    }

    private void BuildUI()
    {
        Root.style.maxWidth = 300;
        Root.Clear();

        var label = new Label("Game UI Initialized");
        Root.Add(label);


        buildingListPanel.ExitMode();
        buildingListPanel.UpdateBuildingList(buildings);
        Root.Add(buildingListPanel);
        inspectPanel.ExitMode();
        Root.Add(inspectPanel);
    }
}
