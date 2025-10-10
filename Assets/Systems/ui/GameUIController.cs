using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlayerGridSelector playerGridSelector;
    [SerializeField] private List<WorldTile> buildings;
    [SerializeField] private BuildManager buildManager;

    [Header("Debug Info")]
    [SerializeField, ReadOnly] private WorldNode selectedNodeA;
    [SerializeField, ReadOnly] private WorldNode selectedNodeB;
    [SerializeField] private UIState currentState = UIState.INSPECTING;

    private VisualElement Root => uiDocument.rootVisualElement;
    private InspectPanel inspectPanel;
    private BuildingSelectionPanel buildingListPanel;

    void Start()
    {
        if (uiDocument == null && TryGetComponent<UIDocument>(out var uiDoc))
        {
            uiDocument = uiDoc;
        }
    }

    void OnEnable()
    {
        BuildUI();
        playerGridSelector.OnNodeSelected += HandleSelection;

    }

    private void HandleSelection(WorldNode node)
    {
        if (node == null) return;

        if (currentState == UIState.BUILDING || currentState == UIState.INSPECTING)
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
        Root.Clear();

        var label = new Label("Game UI Initialized");
        Root.Add(label);

        inspectPanel = new InspectPanel();
        Root.Add(inspectPanel);

        buildingListPanel = new BuildingSelectionPanel();
        Root.Add(buildingListPanel);
        buildingListPanel.UpdateBuildingList(buildings);
    }
}
