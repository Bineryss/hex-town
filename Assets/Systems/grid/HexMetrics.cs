using Systems.Grid;

public class HexMetrics
{
    public readonly static HexCoordinate[] Directions = new HexCoordinate[]
    {
        new(1, 0), new(1, -1), new(0, -1),
        new(-1, 0), new(-1, 1), new(0, 1)
    };
}