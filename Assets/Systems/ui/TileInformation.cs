using System.Collections.Generic;

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
    
    }

    public struct BonusInformation
    {
        public ResourceType ResourceType;
        public float BonusMultiplier;
        public int MaxCapacity;
        public int CurrentInputAmount;
    }
}