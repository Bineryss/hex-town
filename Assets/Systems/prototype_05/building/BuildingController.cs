using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Prototype_04;
using Systems.Prototype_05.UI;

namespace Systems.Prototype_05.Building
{
    public class BuildingController : SerializedMonoBehaviour
    {
        public List<WorldTile> placedBuildings;
        [OdinSerialize] public Dictionary<WorldTile, int> debugInventory;

        private readonly BuildingInventory inventoryRef = BuildingInventory.Instance;

        [Button("Sync inventory")]
        public void SyncInventory()
        {
            inventoryRef.buildingInventory = debugInventory;
            EventBus<BuildingInventoryChanged>.Raise();
        }

        void OnEnable()
        {
            inventoryRef.buildingInventory = debugInventory;
            EventBus<BuildingPlaced>.Event += RemoveBuilding;
        }

        void Start()
        {
            EventBus<BuildingInventoryChanged>.Raise();
        }

        private void RemoveBuilding(BuildingPlaced data)
        {
            inventoryRef.buildingInventory[data.type]--;
            if (inventoryRef.buildingInventory[data.type] == 0)
            {
                inventoryRef.buildingInventory.Remove(data.type);
            }
            EventBus<BuildingInventoryChanged>.Raise();
        }
    }

    public struct BuildingInventoryChanged : IEvent { };
}