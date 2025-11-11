using Systems.Core;
using UnityEngine;

namespace Systems.Prototype_05.Grid
{
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] private Vector2 size;
        [SerializeField] private HexGridGenerator generator;

        public HexGridLayout Layout => layout;
        private HexGridLayout layout;
        private HexGridDS hexGridDS = HexGridDS.Instance;

        void OnEnable()
        {
            layout = new(size, new(transform.position.x, transform.position.z));
            hexGridDS.grid = generator.GenerateGrid();
        }

        public INode GetNode(AxialCoordinate hex)
        {
            if (hexGridDS.grid.TryGetValue(hex, out INode node))
            {
                return node;
            }
            return null;
        }

        public INode GetNode(Vector3 position)
        {
            AxialCoordinate hex = layout.WorldToAxial(position);
            return GetNode(hex);
        }
    }
}