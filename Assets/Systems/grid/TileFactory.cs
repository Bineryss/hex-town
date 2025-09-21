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

        HexTileVariant variant = tile.Variants[0];
        if (variant == null) return null;


        GameObject instance = Instantiate(variant.Prefab, worldPosition, Quaternion.identity, transform);
        instance.name = $"node_{cellPosition}";
        
        MeshFilter meshFilter = instance.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            MeshCollider meshCollider = instance.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        return new Node()
        {
            instance = instance,
            position = cellPosition,
            isWalkable = tile.id != "mountain"
        };
    }
}