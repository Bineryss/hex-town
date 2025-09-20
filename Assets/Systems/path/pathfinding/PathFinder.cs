using System.Collections.Generic;
using System.Linq;
using Systems.Grid;

public class PathFinder
{
    public static List<HexCoordinate> FindPath(Node start, Node end, Dictionary<HexCoordinate, Node> allNodes)
    {
        if (start.isWalkable == false || end.isWalkable == false)
        {
            return new List<HexCoordinate>();
        }

        PathNode startNode = new() { instance = start, G = 0, H = 0 };
        PathNode endNode = new() { instance = end, G = 0, H = 0 };

        startNode.H = startNode.Distance(endNode);

        List<PathNode> toSearch = new() { startNode };
        HashSet<PathNode> processed = new();

        while (toSearch.Any())
        {

            PathNode current = toSearch[0];
            foreach (var node in toSearch)
            {
                if (node.F < current.F || (node.F == current.F && node.H < current.H))
                {
                    current = node;
                }
            }

            if (current.Equals(endNode))
            {
                var currentPathTile = current;
                List<HexCoordinate> path = new();

                while (!currentPathTile.Equals(startNode))
                {
                    path.Add(currentPathTile.Position);
                    currentPathTile = currentPathTile.Connection;
                }
                path.Add(startNode.Position);

                return path;
            }

            processed.Add(current);
            toSearch.Remove(current);

            List<PathNode> neighbors = allNodes[current.Position].Neighbors(allNodes)
            .Where(n => n.isWalkable)
            .Select(n => new PathNode { instance = n, G = 0, H = 0 })
            .Where(n => !processed.Contains(n))
            .ToList();

            foreach (var neighbor in neighbors)
            {
                bool inSearch = toSearch.Contains(neighbor);
                int costToNeighbor = current.G + current.Distance(neighbor);

                if (!inSearch || costToNeighbor < neighbor.G)
                {
                    neighbor.G = costToNeighbor;
                    neighbor.Connection = current;

                    if (!inSearch)
                    {
                        neighbor.H = neighbor.Distance(endNode);
                        toSearch.Add(neighbor);
                    }
                }
            }
        }

        return new();
    }
}