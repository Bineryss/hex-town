using System;
using Systems.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Prototype_04.UI
{
    public class PlayerGridSelector : MonoBehaviour
    {
        public event Action<WorldNode, bool> OnChange;

        [SerializeField] private HexGrid grid;
        private bool wasPressed;
        private AxialCoordinate lastHoveredCoordinate;

        void Update()
        {
            HandleChangeDetection();
        }

        private void HandleChangeDetection()
        {
            WorldNode node = GetNodeUnderMouse();
            bool isPressed = Mouse.current.leftButton.isPressed;
            if (node == null) return;
            if (isPressed == wasPressed && node.Position.Equals(lastHoveredCoordinate)) return;

            wasPressed = isPressed;
            lastHoveredCoordinate = node.Position;
            OnChange?.Invoke(node, isPressed);
        }

        private WorldNode GetNodeUnderMouse()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return null;

            return grid.GetNode(hitInfo.point) as WorldNode;
        }
    }
}