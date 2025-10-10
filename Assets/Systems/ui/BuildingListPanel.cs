using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingListPanel : VisualElement
{
    private ListView buildingListView;
    private List<WorldTile> buildings;

    public BuildingListPanel() : this(new List<WorldTile>())
    {
    }

    public BuildingListPanel(List<WorldTile> buildings)
    {
        this.buildings = buildings;
        CreateHeaderLabels();
        CreateBuildingList();
    }

    private void CreateHeaderLabels()
    {
        var headerContainer = new VisualElement();
        headerContainer.AddToClassList("building-header");
        headerContainer.style.flexDirection = FlexDirection.Row;
        headerContainer.style.paddingBottom = 8;
        headerContainer.style.paddingTop = 8;
        headerContainer.style.paddingLeft = 8;
        headerContainer.style.paddingRight = 8;

        var nameLabel = new Label("Name");
        nameLabel.style.flexGrow = 1;
        nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

        var outputLabel = new Label("Output Resource");
        outputLabel.style.flexGrow = 1;
        outputLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

        var costLabel = new Label("Cost");
        costLabel.style.flexGrow = 1;
        costLabel.style.unityFontStyleAndWeight = FontStyle.Bold;

        headerContainer.Add(nameLabel);
        headerContainer.Add(outputLabel);
        headerContainer.Add(costLabel);

        Add(headerContainer);
    }

    private void CreateBuildingList()
    {
        buildingListView = new ListView
        {
            itemsSource = buildings,
            fixedItemHeight = 30,
            makeItem = MakeBuildingItem,
            bindItem = BindBuildingItem,
            selectionType = SelectionType.Single
        };

        buildingListView.style.flexGrow = 1;
        Add(buildingListView);
    }

    private VisualElement MakeBuildingItem()
    {
        var itemContainer = new VisualElement();
        itemContainer.style.flexDirection = FlexDirection.Row;
        itemContainer.style.paddingLeft = 8;
        itemContainer.style.paddingRight = 8;

        var nameLabel = new Label();
        nameLabel.style.flexGrow = 1;
        nameLabel.AddToClassList("building-name");

        var resourceLabel = new Label();
        resourceLabel.style.flexGrow = 1;
        resourceLabel.AddToClassList("building-resource");

        var costLabel = new Label();
        costLabel.style.flexGrow = 1;
        costLabel.AddToClassList("building-cost");

        itemContainer.Add(nameLabel);
        itemContainer.Add(resourceLabel);
        itemContainer.Add(costLabel);

        return itemContainer;
    }

    private void BindBuildingItem(VisualElement element, int index)
    {
        if (index < 0 || index >= buildings.Count)
            return;

        var building = buildings[index];
        var labels = element.Query<Label>().ToList();

        if (labels.Count >= 3)
        {
            labels[0].text = building.name;
            labels[1].text = building.resourceType.ToString();
            labels[2].text = CalculateCost(building).ToString();
        }
    }

    private int CalculateCost(WorldTile building)
    {
        // Implement cost calculation logic based on your game rules
        // This is a placeholder implementation
        return -1;
    }

    public void UpdateBuildings(List<WorldTile> newBuildings)
    {
        buildings = newBuildings;
        buildingListView.itemsSource = buildings;
        buildingListView.Rebuild();
    }
}
