using System.Collections.Generic;
using Systems.Grid;
using UnityEngine;


public class GameTileFactory : MonoBehaviour, ITileFactory
{
    [SerializeField] private List<HexTile> hexTiles;


    public Node CreateTile(HexCoordinate cellPosition, Vector3 worldPosition)
    {
        HexTile tile = hexTiles[Random.Range(0, hexTiles.Count)];
        if (tile == null || tile.Variants == null || tile.Variants.Count == 0) return null;

        HexTileVariant variant = tile.Variants[Random.Range(0, tile.Variants.Count)];
        if (variant == null) return null;


        GameObject instance = Instantiate(variant.Prefab, worldPosition, Quaternion.identity, transform);
        instance.name = $"node_{cellPosition}";
        return new Node(instance, cellPosition);
    }
}