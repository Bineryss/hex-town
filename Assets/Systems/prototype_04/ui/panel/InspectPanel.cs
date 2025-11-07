using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_04.UI
{
    public class InspectPanel : VisualElement, IUIModeSegment
    {
        private readonly Label titleELement;
        private readonly Label selectedTileNameElement;
        private readonly Label selectedTileProductionTypeElement;
        private readonly Label selectedTileProductionRateElement;
        private readonly Label selectedTileAvailableResourcesElement;
        private readonly Label selectedTileCumulatedBonusElement;

        private readonly VisualElement bonusContainer = new();
        private readonly List<Label> bonusLabels = new();

        private readonly VisualElement subTileContainer = new();
        private readonly List<Label> subTileLabels = new();

        public InspectPanel(ResourceOverview resourceOverview)
        {
            style.width = 300;
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
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

            selectedTileCumulatedBonusElement = new Label("Cumulated Bonus: N/A");
            selectedTileCumulatedBonusElement.style.color = Color.white;
            selectedTileCumulatedBonusElement.style.fontSize = 14;
            selectedTileCumulatedBonusElement.style.marginBottom = 5;
            Add(selectedTileCumulatedBonusElement);

            Add(bonusContainer);
            Add(subTileContainer);
            Add(resourceOverview);
        }

        public void EnterMode()
        {
            style.display = DisplayStyle.Flex;
        }

        public void ExitMode()
        {
            style.display = DisplayStyle.None;
        }

        public void UpdateTileInfo(TileInformation tileInfo)
        {
            selectedTileNameElement.text = $"Tile: {tileInfo.TileName}";
            selectedTileProductionTypeElement.text = $"Production Type: {tileInfo.ProductionType}";
            selectedTileProductionRateElement.text = $"Production Rate: {tileInfo.ProductionRate}";
            selectedTileAvailableResourcesElement.text = $"Available Resources: {tileInfo.AvailableResources}";
            selectedTileCumulatedBonusElement.text = $"Cumulated Bonus: {tileInfo.CumulatedBonus}%";
            UpdateBonusLabels(tileInfo.BonusInformations);
            UpdateSubTileLabels(tileInfo.SubTiles);
        }

        private void UpdateBonusLabels(List<BonusInformation> bonusInfos)
        {
            bonusContainer.Clear();
            bonusLabels.Clear();

            foreach (var bonusInfo in bonusInfos)
            {
                Label bonusLabel = new($"Bonus: {bonusInfo.ResourceType} - Multiplier: {bonusInfo.BonusMultiplier} - Max Capacity: {bonusInfo.MaxCapacity} - Current Input: {bonusInfo.CurrentInputAmount}");
                bonusLabel.style.color = Color.white;
                bonusLabel.style.fontSize = 12;
                bonusLabel.style.marginBottom = 3;
                bonusContainer.Add(bonusLabel);
                bonusLabels.Add(bonusLabel);
            }
        }

        private void UpdateSubTileLabels(List<SubTile> subTiles)
        {
            subTileContainer.Clear();
            subTileLabels.Clear();

            foreach (var subTile in subTiles)
            {
                Label subTileLabel = new($"Sub-Tile: {subTile.Position} - Type: {subTile.Type} - Production: {subTile.Production}");
                subTileLabel.style.color = Color.white;
                subTileLabel.style.fontSize = 12;
                subTileLabel.style.marginBottom = 3;
                subTileContainer.Add(subTileLabel);
                subTileLabels.Add(subTileLabel);
            }
        }
    }
}