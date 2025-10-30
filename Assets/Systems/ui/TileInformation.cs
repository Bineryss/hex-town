using System.Collections.Generic;
using Systems.Grid;

namespace Systems.UI
{
    public struct TileInformation
    {
        public string TileName;
        public ResourceType ProductionType;
        public int ProductionRate;
        public int AvailableResources;
        public List<BonusInformation> BonusInformations;
        public float CumulatedBonus;
        public List<SubTile> SubTiles;

    }

    public struct BonusInformation
    {
        public ResourceType ResourceType;
        public float BonusMultiplier;
        public int MaxCapacity;
        public int CurrentInputAmount;
    }

    public struct SubTile
    {
        public HexCoordinate Position;
        public ResourceType Type;
        public int Production;
    }
}