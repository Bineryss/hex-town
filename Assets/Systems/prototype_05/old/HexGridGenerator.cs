using System.Collections.Generic;
using Sirenix.OdinInspector;
using Systems.Core;
using Systems.Prototype_05.Grid;
using UnityEngine;

namespace Systems.Prototype_05
{
    public class HexGridGenerator : SerializedMonoBehaviour
    {
        public HexGrid grid;
        public int gridRadius = 5;

        [SerializeField] private ITileFactory tileFactory;

        public Dictionary<AxialCoordinate, INode> nodes;
        public HexGridLayout layout;

        public Dictionary<AxialCoordinate, INode> GenerateGrid()
        {
            nodes ??= new Dictionary<AxialCoordinate, INode>();
            if (grid == null) return new();
            layout = grid.Layout;


            tileFactory.PregenerateTiles(gridRadius);
            tileFactory.SetParent(grid.transform);

            for (int q = -gridRadius; q <= gridRadius; q++)
            {
                for (int r = Mathf.Max(-gridRadius, -q - gridRadius); r <= Mathf.Min(gridRadius, -q + gridRadius); r++)
                {
                    CreateCell(q, r);
                }
            }
            return nodes;
        }

        private void CreateCell(int q, int r)
        {
            AxialCoordinate axialCoordinate = new(q, r);
            Vector3 worldPos = layout.AxialToWorld(axialCoordinate);
            INode instance = tileFactory.CreateTile(axialCoordinate, worldPos);
            if (instance != null)
            {
                nodes[axialCoordinate] = instance;
            }
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