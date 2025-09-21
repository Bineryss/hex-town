using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using UnityEngine;

[RequireComponent(typeof(GridLayout))]
public class HexGridGenerator : SerializedMonoBehaviour
{
    public GridLayout grid;
    public int gridRadius = 5;

    [SerializeField] private ITileFactory tileFactory;

    [OdinSerialize] public Dictionary<HexCoordinate, Node> nodes = new();

    public Dictionary<HexCoordinate, Node> GenerateGrid()
    {
        if (grid == null) grid = GetComponent<GridLayout>();

        for (int y = -gridRadius; y <= gridRadius; y++)
        {
            for (int x = -gridRadius; x <= gridRadius; x++)
            {
                var cell = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.CellToWorld(cell);
                HexCoordinate hexCoord = HexCoordinate.FromOffsetCoordinates(cell.x, cell.y);
                Node instance = tileFactory.CreateTile(hexCoord, worldPos);
                if (instance != null)
                {
                    nodes[hexCoord] = instance;
                }
            }
        }
        return nodes;
    }

    [ContextMenu("Generate Test Grid")]
    public void GenerateTestGrid()
    {
        // Clear existing children
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }
        foreach (GameObject child in children)
        {
            DestroyImmediate(child); // Use DestroyImmediate(child) if in editor context
        }
        nodes.Clear();
        GenerateGrid();
    }

}