using System.Collections.Generic;
using Systems.Prototype_04;
using Unity.VisualScripting;

namespace Systems.Prototype_05.Building
{
    public class BuildingInventory
    {
        public static BuildingInventory Instance => instance ??= new BuildingInventory();
        private static BuildingInventory instance;
        public int PacksLeft;
        public Dictionary<WorldTile, int> buildingInventory = new();
    }
}