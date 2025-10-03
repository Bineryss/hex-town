using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using UnityEngine;

[RequireComponent(typeof(GridLayout))]
public class HexGrid : SerializedMonoBehaviour
{
    [SerializeField] private GridLayout grid;
    [SerializeField] private HexGridGenerator generator;
    [OdinSerialize, ReadOnly] private Dictionary<HexCoordinate, INode> nodes = new();
    public HexCoordinate WorldToHex(Vector3 position)
    {
        Vector3Int cell = grid.WorldToCell(position);
        return HexCoordinate.FromOffsetCoordinates(cell.x, cell.y);
    }

    public INode GetNode(HexCoordinate hex)
    {
        if (nodes.TryGetValue(hex, out INode node))
        {
            return node;
        }
        return null;
    }

    public INode GetNode(Vector3 position)
    {
        HexCoordinate hex = WorldToHex(position);
        return GetNode(hex);
    }

    public void Initialize()
    {
        nodes = generator.GenerateGrid();
    }

    public void Awake()
    {
        Initialize();
    }
}
