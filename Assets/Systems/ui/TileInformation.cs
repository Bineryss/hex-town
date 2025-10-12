namespace Systems.UI
{
    public struct TileInformation
    {
        public string TileName;
        public ResourceType ProductionType;
        public int ProductionRate;
        public int AvailableResources;
        public ResourceType[] AcceptedResources;
    }
}