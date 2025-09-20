using Systems.Grid;

public class PathNode
{
    public PathNode Connection { get; set; }
    public int G { get; set; } // Cost from start to this node
    public int H { get; set; } // Heuristic cost from this node to end
    public int F => G + H; // Total cost
    public HexCoordinate Position { get; set; }

    public int Distance(PathNode other)
    {
        return Position.Distance(other.Position);
    }

    public override bool Equals(object obj)
    {
        if (obj is PathNode other)
        {
            return Position.Equals(other.Position);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}