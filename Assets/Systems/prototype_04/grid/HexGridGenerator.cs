using System.Collections.Generic;
using Sirenix.OdinInspector;
using Systems.Core;
using UnityEngine;

namespace Systems.Prototype_04
{
    [RequireComponent(typeof(GridLayout))]
    public class HexGridGenerator : SerializedMonoBehaviour
    {
        public GridLayout grid;
        public int gridRadius = 5;

        [SerializeField] private ITileFactory tileFactory;

        public Dictionary<AxialCoordinate, INode> nodes;

        public Dictionary<AxialCoordinate, INode> GenerateGrid()
        {
            if (nodes == null) nodes = new Dictionary<AxialCoordinate, INode>();
            if (grid == null) grid = GetComponent<GridLayout>();

            tileFactory.PregenerateTiles(gridRadius);
            tileFactory.SetParent(grid.transform);

            for (int y = -gridRadius; y <= gridRadius; y++)
            {
                for (int x = -gridRadius; x <= gridRadius; x++)
                {
                    var cell = new Vector3Int(x, y, 0);
                    Vector3 worldPos = grid.CellToWorld(cell);
                    AxialCoordinate hexCoord = AxialCoordinate.FromOffsetCoordinates(cell.x, cell.y);
                    INode instance = tileFactory.CreateTile(hexCoord, worldPos);
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
}