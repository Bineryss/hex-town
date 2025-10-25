using System;
using Systems.Grid;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.UI
{
    public class PlayerGridSelector : MonoBehaviour
    {
        public Action<WorldNode> OnNodeSelected;
        public Action<WorldNode, bool> OnChange;


        private bool wasPressed;
        private HexCoordinate lastHoveredCoordinate;

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseClick();
            }

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

        private void HandleMouseClick()
        {
            WorldNode node = GetNodeUnderMouse();
            if (node == null) return;

            OnNodeSelected?.Invoke(node);
        }

        private WorldNode GetNodeUnderMouse()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return null;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
            return hitInfo.transform.GetComponentInParent<WorldNode>();
        }
    }
}