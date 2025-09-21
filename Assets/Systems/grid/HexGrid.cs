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
    [OdinSerialize] private Dictionary<HexCoordinate, Node> nodes = new();
    public HexCoordinate WorldToHex(Vector3 position)
    {
        Vector3Int cell = grid.WorldToCell(position);
        return HexCoordinate.FromOffsetCoordinates(cell.x, cell.y);
    }

    public Node GetNode(HexCoordinate hex)
    {
        if (nodes.TryGetValue(hex, out Node node))
        {
            return node;
        }
        return null;
    }

    public Node GetNode(Vector3 position)
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
