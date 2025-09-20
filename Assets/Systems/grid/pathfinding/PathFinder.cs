using System.Collections.Generic;
using System.Linq;
using Systems.Grid;
using UnityEngine;

public class PathFinder
{
    public static List<HexCoordinate> FindPath(Node start, Node end, Dictionary<HexCoordinate, Node> allNodes)
    {
        Debug.Log(start.isWalkable);
        if (start.isWalkable == false || end.isWalkable == false)
        {
            return new List<HexCoordinate>();
        }

        PathNode startNode = new() { Position = start.position, G = 0, H = 0 };
        PathNode endNode = new() { Position = end.position, G = 0, H = 0 };
        List<PathNode> toSearch = new() { startNode };
        HashSet<PathNode> processed = new();

        while (toSearch.Any())
        {
            Debug.Log("Searching");
            PathNode current = toSearch[0];
            foreach (var node in toSearch)
            {
                if (node.F < current.F || (node.F == current.F && node.H < current.H))
                {
                    current = node;
                }
            }

            if (current == endNode)
            {
                Debug.Log("found");
                var currentPathTile = endNode;
                List<HexCoordinate> path = new();

                while (currentPathTile != startNode)
                {
                    path.Add(currentPathTile.Position);
                    currentPathTile = currentPathTile.Connection;
                }
                return path;
            }

            processed.Add(current);
            toSearch.Remove(current);

            List<PathNode> neighbors = allNodes[current.Position].Neighbors(allNodes).Where(n => n.isWalkable).Select(n => new PathNode { Position = n.position, G = 0, H = 0 }).ToList();

            foreach (var neighbor in neighbors)
            {
                Debug.Log($"neighbor: {neighbor.Position}");
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





        // Implement A* pathfinding algorithm
        return new List<HexCoordinate>();
    }
}