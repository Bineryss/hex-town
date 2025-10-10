using UnityEngine;
using UnityEngine.UIElements;

public class InspectPanel : VisualElement
{
    private readonly Label titleELement;
    private readonly Label selectedTileNameElement;
    private readonly Label selectedTileProductionTypeElement;
    private readonly Label selectedTileProductionRateElement;
    private readonly Label selectedTileAvailableResourcesElement;
    private readonly Label selectedTileAcceptedResourcesElement;

    public InspectPanel()
    {
        style.width = 250;
        style.height = 300;
        style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        style.flexDirection = FlexDirection.Column;
        style.paddingTop = 10;
        style.paddingLeft = 10;
        style.paddingRight = 10;
        style.paddingBottom = 10;
        style.justifyContent = Justify.FlexStart;
        style.alignItems = Align.FlexStart;

        titleELement = new Label("Inspect Panel");
        titleELement.style.color = Color.white;
        titleELement.style.fontSize = 18;
        titleELement.style.unityFontStyleAndWeight = FontStyle.Bold;
        titleELement.style.marginBottom = 10;
        Add(titleELement);

        selectedTileNameElement = new Label("Tile: None");
        selectedTileNameElement.style.color = Color.white;
        selectedTileNameElement.style.fontSize = 14;
        selectedTileNameElement.style.marginBottom = 5;
        Add(selectedTileNameElement);

        selectedTileProductionTypeElement = new Label("Production Type: N/A");
        selectedTileProductionTypeElement.style.color = Color.white;
        selectedTileProductionTypeElement.style.fontSize = 14;
        selectedTileProductionTypeElement.style.marginBottom = 5;
        Add(selectedTileProductionTypeElement);

        selectedTileProductionRateElement = new Label("Production Rate: N/A");
        selectedTileProductionRateElement.style.color = Color.white;
        selectedTileProductionRateElement.style.fontSize = 14;
        selectedTileProductionRateElement.style.marginBottom = 5;
        Add(selectedTileProductionRateElement);

        selectedTileAvailableResourcesElement = new Label("Available Resources: N/A");
        selectedTileAvailableResourcesElement.style.color = Color.white;
        selectedTileAvailableResourcesElement.style.fontSize = 14;
        selectedTileAvailableResourcesElement.style.marginBottom = 5;
        Add(selectedTileAvailableResourcesElement);

        selectedTileAcceptedResourcesElement = new Label("Accepted Resources: N/A");
        selectedTileAcceptedResourcesElement.style.color = Color.white;
        selectedTileAcceptedResourcesElement.style.fontSize = 14;
        selectedTileAcceptedResourcesElement.style.marginBottom = 5;
        Add(selectedTileAcceptedResourcesElement);
    }

    public void UpdateTileInfo(TileInformation tileInfo)
    {
        selectedTileNameElement.text = $"Tile: {tileInfo.TileName}";
        selectedTileProductionTypeElement.text = $"Production Type: {tileInfo.ProductionType}";
        selectedTileProductionRateElement.text = $"Production Rate: {tileInfo.ProductionRate}";
        selectedTileAvailableResourcesElement.text = $"Available Resources: {tileInfo.AvailableResources}";
        selectedTileAcceptedResourcesElement.text = $"Accepted Resources: {string.Join(", ", tileInfo.AcceptedResources)}";
    }
}