namespace Systems.Core
{
    public static class HexMetrics
    {
        public readonly static AxialCoordinate[] Directions = new AxialCoordinate[]
        {
        new(1, 0), new(1, -1), new(0, -1),
        new(-1, 0), new(-1, 1), new(0, 1)
        };
    }
}