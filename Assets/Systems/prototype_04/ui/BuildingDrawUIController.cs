using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Systems.Prototype_04.UI
{
    public class BuildingDrawUIController : SerializedMonoBehaviour, IUIController
    {
        public IUIModeSegment UIModeSegment => buildingDrawPanel;

        [SerializeField] private List<BuildingPack> buildingPacks;
        [OdinSerialize] public Dictionary<WorldTile, int> buildingInventory;
        [SerializeField] private int pointsThreshold;
        [SerializeField] private BuildingUIController buildingUIController; //TODO extract to general score thing

        public int overallPoints;
        public int currentPoints;
        public int availableDraws;
        private int currentDrawCycle;
        private BuildingDrawPanel buildingDrawPanel;

        public void Activate()
        {
            buildingDrawPanel.EnterMode();
        }

        void Update()
        {
            overallPoints = Mathf.FloorToInt(buildingUIController.PlacedBuildings
                .Select(b => new { Type = b.Resource.scoreType, Quantity = b.GetAvailableProduction() * b.Resource.conversionRate })
                .GroupBy(e => e.Type)
                .Select(g => g.Sum(b => b.Quantity))
                .Sum());
            currentPoints = overallPoints - pointsThreshold * currentDrawCycle;
            Debug.Log($"overall: {overallPoints}, threshhold: {pointsThreshold}, cycle: {currentDrawCycle}, current: {currentPoints}");

            if (overallPoints > pointsThreshold * (currentDrawCycle + 1))
            {
                currentDrawCycle++;
                availableDraws++;
            }
            if (availableDraws > 0)
            {
                Activate();
            }
        }

        private void HandleBuildingPackSelected(string packName)
        {
            Debug.Log($"Building pack selected: {packName}");
            // Handle building pack selection logic here
            var selectedPack = buildingPacks.FirstOrDefault(pack => pack.packName == packName);
            if (selectedPack != null)
            {
                // Update the building inventory with the selected pack's buildings
                foreach (var building in selectedPack.buildings)
                {
                    buildingInventory[building.building] = Random.Range(building.min, building.max + 1) + buildingInventory.GetValueOrDefault(building.building, 0);
                }
            }
            availableDraws--;
            if (availableDraws == 0)
            {
                Exit();
            }
        }

        public void Exit()
        {
            buildingDrawPanel.ExitMode();
        }

        public void HandleMouseInteraction(WorldNode node, WorldNode prevNode, bool isClick)
        {
            // No mouse interaction needed for building draw mode
        }

        public void Initialize()
        {
            buildingInventory = new Dictionary<WorldTile, int>();
            buildingDrawPanel = new BuildingDrawPanel();
            buildingDrawPanel.SetCurrentDeckOptions(buildingPacks.ConvertAll(pack => new BuildingDrawOption
            {
                Id = pack.packName,
                Label = pack.packName
            }));
            buildingDrawPanel.OnBuildingPackSelected += HandleBuildingPackSelected;
        }
    }
}