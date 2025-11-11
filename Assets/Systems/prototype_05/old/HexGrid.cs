using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using UnityEngine;

namespace Systems.Prototype_05
{
    [RequireComponent(typeof(GridLayout))]
    public class HexGrid : SerializedMonoBehaviour
    {
        [SerializeField] private GridLayout grid;
        public GridLayout Grid => grid;
        [SerializeField] private HexGridGenerator generator;
        [OdinSerialize, ReadOnly] private Dictionary<AxialCoordinate, INode> nodes = new();
        public AxialCoordinate WorldToHex(Vector3 position)
        {
            Vector3Int cell = grid.WorldToCell(position);
            return AxialCoordinate.FromOffsetCoordinates(cell.x, cell.y);
        }

        public INode GetNode(AxialCoordinate hex)
        {
            if (nodes.TryGetValue(hex, out INode node))
            {
                return node;
            }
            return null;
        }

        public INode GetNode(Vector3 position)
        {
            AxialCoordinate hex = WorldToHex(position);
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
}