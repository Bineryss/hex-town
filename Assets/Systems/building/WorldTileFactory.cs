using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using UnityEngine;

public class WorldTileFactory : SerializedMonoBehaviour, ITileFactory
{
    [OdinSerialize] private Dictionary<WorldType, WorldTile> worldTiles;
    [SerializeField] private WorldNode tilePrefab;
    [SerializeField] private WorldType defaultTileType = WorldType.Grassland;

    [SerializeField] private int mountainCount = 3;
    [SerializeField] private int mountainSize = 2;
    [SerializeField] private int mountainSpread = 2;
    [SerializeField] private AnimationCurve mountainToForestCurve;

    private Dictionary<HexCoordinate, WorldType> tileMap = new();

    public INode CreateTile(HexCoordinate cellPosition, Vector3 worldPosition)
    {
        WorldNode instance = Instantiate(tilePrefab, worldPosition, Quaternion.identity);
        instance.name = $"node_{cellPosition}";

        if (tileMap.TryGetValue(cellPosition, out WorldType type))
        {
            instance.worldTile = worldTiles[type];
        }
        else
        {
            instance.worldTile = worldTiles[defaultTileType];
        }

        instance.Position = cellPosition;

        return instance;
    }

    public void PregenerateTiles(int size)
    {
        tileMap.Clear();
        var mountainPositions = SeedMountains(mountainCount, size);
        GenerateMountains(mountainPositions, mountainSize);
    }

    private List<HexCoordinate> SeedMountains(int count, int gridRadius)
    {
        List<HexCoordinate> mountainPositions = new();

        for (int i = 0; i < count; i++)
        {
            int q = Random.Range(-gridRadius, gridRadius + 1);
            int r = Random.Range(-gridRadius, gridRadius + 1);
            HexCoordinate coord = new(q, r);

            if (!tileMap.ContainsKey(coord))
            {
                tileMap[coord] = WorldType.Mountain;
                mountainPositions.Add(coord);
            }
        }

        return mountainPositions;
    }
    private void GenerateMountains(List<HexCoordinate> mountainPositions, int range)
    {
        int size = range;

        foreach (var pos in mountainPositions)
        {
            for (int dq = -size; dq <= size; dq++)
            {
                for (int dr = -size; dr <= size; dr++)
                {
                    HexCoordinate neighbor = new(pos.Q + dq, pos.R + dr);
                    if (!tileMap.ContainsKey(neighbor))
                    {
                        int distance = neighbor.Distance(pos);
                        float remaped = Mathf.InverseLerp(0, range, distance);
                        float whatIsIt = mountainToForestCurve.Evaluate(remaped);

                        if (whatIsIt > 0.5f)
                        {
                            if (Random.Range(0f, 1f) > whatIsIt)
                            {

                                tileMap[neighbor] = WorldType.Forest;
                            }
                            else
                            {
                                tileMap[neighbor] = WorldType.Grassland;
                            }
                        }
                        else
                        {
                            if (Random.Range(0f, .6f) > whatIsIt)
                            {

                                tileMap[neighbor] = WorldType.Mountain;
                            }
                            else
                            {
                                tileMap[neighbor] = WorldType.Forest;
                            }
                        }
                    }
                }
            }
        }
    }

    public enum WorldType
    {
        Grassland,
        Forest,
        Mountain,
        Water
    }
}