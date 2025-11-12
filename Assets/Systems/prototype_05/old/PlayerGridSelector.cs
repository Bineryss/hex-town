using System;
using Systems.Core;
using Systems.Prototype_05.Grid;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class PlayerGridSelector : MonoBehaviour
    {
        public event Action<WorldNode, bool> OnChange;

        [SerializeField] private HexGrid grid;
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private Plane groundPlane;
        [SerializeField] private float planeHeight;
        private bool wasPressed;
        private AxialCoordinate? lastHoveredCoordinate;

        private bool pointerBlocked;

        void OnEnable()
        {
            EventBus<PointerOverUIElement>.Event += (data) => pointerBlocked = data.Blocking;
            groundPlane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));
        }

        void Update()
        {
            HandleChangeDetection();
        }

        private void HandleChangeDetection()
        {
            if (pointerBlocked)
            {
                if (lastHoveredCoordinate == null) return;
                lastHoveredCoordinate = null;
                OnChange?.Invoke(null, false);
                Debug.Log($"pointer blocked");
                return;
            }

            WorldNode node = GetNodeUnderMouse();
            bool isPressed = Mouse.current.leftButton.isPressed;
            if (node == null)
            {
                if (lastHoveredCoordinate == null) return;
                lastHoveredCoordinate = null;
                Debug.Log($"node is actually null");
                OnChange?.Invoke(null, false);
                return;
            }
            if (isPressed == wasPressed && node.Position.Equals(lastHoveredCoordinate)) return;

            wasPressed = isPressed;
            lastHoveredCoordinate = node.Position;
            OnChange?.Invoke(node, isPressed);
        }

        private WorldNode GetNodeUnderMouse()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!groundPlane.Raycast(ray, out float enter)) return null;

            return grid.GetNode(ray.GetPoint(enter)) as WorldNode;
        }
    }
}