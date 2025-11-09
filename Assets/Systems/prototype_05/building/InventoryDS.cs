using System.Collections.Generic;
using Systems.Prototype_04;

namespace Systems.Prototype_05.Building
{
    public class InventoryDS
    {
        public static InventoryDS Instance => instance ??= new InventoryDS();
        private static InventoryDS instance;
        public int PacksLeft => packsLeft;
        public int packsLeft;
        public Dictionary<WorldTile, int> BuildingInventory => buildingInventory;
        public readonly Dictionary<WorldTile, int> buildingInventory = new();
    }
}