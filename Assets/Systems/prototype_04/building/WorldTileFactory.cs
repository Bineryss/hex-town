using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using UnityEngine;

namespace Systems.Prototype_04
{

    public class WorldTileFactory : SerializedMonoBehaviour, ITileFactory
    {
        [OdinSerialize] private Dictionary<WorldType, WorldTile> worldTiles;
        [SerializeField] private WorldNode tilePrefab;
        [SerializeField] private WorldType defaultTileType = WorldType.Grassland;

        [SerializeField] private int mountainCount = 3;
        [SerializeField] private int mountainSize = 2;
        [SerializeField] private AnimationCurve mountainToForestCurve;

        private Transform parentTransform;
        private Dictionary<AxialCoordinate, WorldType> tileMap = new();

        public void SetParent(Transform parent)
        {
            parentTransform = parent;
        }
        public INode CreateTile(AxialCoordinate cellPosition, Vector3 worldPosition)
        {
            WorldNode instance = Instantiate(tilePrefab, worldPosition, Quaternion.identity, parentTransform);
            instance.name = $"{instance.ResourceType}_{cellPosition}";

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

        private List<AxialCoordinate> SeedMountains(int count, int gridRadius)
        {
            List<AxialCoordinate> mountainPositions = new();

            for (int i = 0; i < count; i++)
            {
                int q = Random.Range(-gridRadius, gridRadius + 1);
                int r = Random.Range(-gridRadius, gridRadius + 1);
                AxialCoordinate coord = new(q, r);

                if (!tileMap.ContainsKey(coord))
                {
                    tileMap[coord] = WorldType.Mountain;
                    mountainPositions.Add(coord);
                }
            }

            return mountainPositions;
        }
        private void GenerateMountains(List<AxialCoordinate> mountainPositions, int range)
        {
            int size = range;

            foreach (var pos in mountainPositions)
            {
                for (int dq = -size; dq <= size; dq++)
                {
                    for (int dr = -size; dr <= size; dr++)
                    {
                        AxialCoordinate neighbor = new(pos.Q + dq, pos.R + dr);
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
}