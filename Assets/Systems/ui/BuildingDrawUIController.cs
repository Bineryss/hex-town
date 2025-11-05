using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Systems.UI
{
    public class BuildingDrawUIController : SerializedMonoBehaviour, IUIController
    {
        public IUIModeSegment UIModeSegment => buildingDrawPanel;

        [SerializeField] private List<BuildingPack> buildingPacks;
        [OdinSerialize] public Dictionary<WorldTile, int> buildingInventory;

        private BuildingDrawPanel buildingDrawPanel;

        public void Activate()
        {
            buildingDrawPanel.EnterMode();
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
                    if (buildingInventory.ContainsKey(building))
                    {
                        buildingInventory[building]++;
                    }
                    else
                    {
                        buildingInventory[building] = 1;
                    }
                }
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