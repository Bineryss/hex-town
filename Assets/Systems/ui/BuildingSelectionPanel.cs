using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingSelectionPanel : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BuildingSelectionPanel, UxmlTraits> { }

    public event Action<WorldTile> OnBuildingSelected;

    private VisualElement detailContainer;
    private Label tileName;
    private Label productionType;
    private Label productionRate;
    private Label availableResources;
    private Label acceptedResources;

    private ListView buildingListView;
    private List<WorldTile> buildings;
    private WorldTile selectedBuilding;

    public BuildingSelectionPanel() : this(new List<WorldTile>())
    {
    }

    public BuildingSelectionPanel(List<WorldTile> availableBuildings)
    {
        buildings = availableBuildings;
        
        CreateDetailZone();
        CreateListZone();
        
        SetDefaultDetailState();
    }

    private void CreateDetailZone()
    {
        detailContainer = new VisualElement();
        detailContainer.style.paddingTop = 10;
        detailContainer.style.paddingBottom = 10;
        detailContainer.style.paddingLeft = 15;
        detailContainer.style.paddingRight = 15;
        detailContainer.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);

        var header = new Label("Inspect Panel");
        header.style.fontSize = 16;
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.style.marginBottom = 10;
        header.style.color = Color.white;
        detailContainer.Add(header);

        tileName = CreateDetailLabel("Tile: None");
        productionType = CreateDetailLabel("Production Type: N/A");
        productionRate = CreateDetailLabel("Production Rate: N/A");
        availableResources = CreateDetailLabel("Available Resources: N/A");
        acceptedResources = CreateDetailLabel("Accepted Resources: N/A");

        Add(detailContainer);
    }

    private Label CreateDetailLabel(string text)
    {
        var label = new Label(text);
        label.style.fontSize = 12;
        label.style.marginBottom = 5;
        label.style.color = Color.white;
        detailContainer.Add(label);
        return label;
    }

    private void CreateListZone()
    {
        var listContainer = new VisualElement();
        listContainer.style.flexGrow = 1;
        listContainer.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        listContainer.style.paddingTop = 10;
        listContainer.style.paddingBottom = 10;

        var headerRow = CreateHeaderRow();
        listContainer.Add(headerRow);

        buildingListView = new ListView
        {
            itemsSource = buildings,
            fixedItemHeight = 30,
            makeItem = MakeBuildingListItem,
            bindItem = BindBuildingListItem,
            selectionType = SelectionType.Single
        };

        buildingListView.selectionChanged += OnListSelectionChanged;
        buildingListView.style.flexGrow = 1;
        buildingListView.style.marginTop = 5;

        listContainer.Add(buildingListView);
        Add(listContainer);
    }

    private VisualElement CreateHeaderRow()
    {
        var headerRow = new VisualElement();
        headerRow.style.flexDirection = FlexDirection.Row;
        headerRow.style.paddingLeft = 15;
        headerRow.style.paddingRight = 15;
        headerRow.style.paddingBottom = 5;
        headerRow.style.borderBottomWidth = 1;
        headerRow.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        var nameHeader = CreateHeaderLabel("Name");

        headerRow.Add(nameHeader);

        return headerRow;
    }

    private Label CreateHeaderLabel(string text)
    {
        var label = new Label(text);
        label.style.fontSize = 12;
        label.style.color = Color.white;
        label.style.unityFontStyleAndWeight = FontStyle.Bold;
        return label;
    }

    private VisualElement MakeBuildingListItem()
    {
        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.paddingLeft = 15;
        row.style.paddingRight = 15;
        row.style.paddingTop = 5;
        row.style.paddingBottom = 5;

        var nameLabel = new Label();
        nameLabel.style.fontSize = 12;
        nameLabel.style.color = Color.white;

        row.Add(nameLabel);

        return row;
    }

    private void BindBuildingListItem(VisualElement element, int index)
    {
        if (index < 0 || index >= buildings.Count)
            return;

        var building = buildings[index];
        var nameLabel = element.Q<Label>();

        if (nameLabel != null)
        {
            nameLabel.text = building.name;
        }
    }

    private void OnListSelectionChanged(IEnumerable<object> selectedItems)
    {
        foreach (var item in selectedItems)
        {
            if (item is WorldTile tile)
            {
                selectedBuilding = tile;
                UpdateDetailView(tile);
                OnBuildingSelected?.Invoke(tile);
                break;
            }
        }
    }

    private void UpdateDetailView(WorldTile tile)
    {
        tileName.text = $"Tile: {tile.name}";
        productionType.text = $"Production Type: {tile.resourceType}";
        productionRate.text = $"Production Rate: {tile.resourceAmount}";
        
        var tradeableList = tile.TradeableResources.Count > 0 
            ? string.Join(", ", tile.TradeableResources) 
            : "N/A";
        availableResources.text = $"Available Resources: {tradeableList}";
        
        var acceptedList = tile.inputBonuses.Count > 0 
            ? string.Join(", ", tile.inputBonuses.ConvertAll(b => b.input.ToString())) 
            : "N/A";
        acceptedResources.text = $"Accepted Resources: {acceptedList}";
    }

    private void SetDefaultDetailState()
    {
        tileName.text = "Tile: None";
        productionType.text = "Production Type: N/A";
        productionRate.text = "Production Rate: N/A";
        availableResources.text = "Available Resources: N/A";
        acceptedResources.text = "Accepted Resources: N/A";
    }

    public void UpdateBuildingList(List<WorldTile> newBuildings)
    {
        buildings = newBuildings;
        buildingListView.itemsSource = buildings;
        buildingListView.Rebuild();
    }

    public WorldTile GetSelectedBuilding()
    {
        return selectedBuilding;
    }
}
