using System.Collections.Generic;
using Systems.Transport;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerState currentState = PlayerState.EXPLORING;
        [SerializeField] private SmoothLineRenderer smoothLineRendererPrefab;
        [SerializeField] private float offsetHeight = 0.4f;
        [Header("Managers")]
        [SerializeField] private HexGrid hexGrid;
        [SerializeField] private BuildManager buildManager;
        [SerializeField] private TransportManager transportManager;

        [Header("Debug Info")]
        [SerializeField] private WorldNode selectedNodeA;
        [SerializeField] private WorldNode selectedNodeB;

        private readonly List<SmoothLineRenderer> activeLines = new();

        void Awake()
        {
            Keyboard.current.onTextInput += ctx => HandleKeyPressed(ctx);
        }

        void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleMouseClick();
            }
        }


        private void HandleKeyPressed(char key)
        {
            HandleModeSelection(key);

            if (currentState == PlayerState.BUILDING)
            {
                HandleBuildingInput(key);
            }
            else if (currentState == PlayerState.MANAGING_TRANSPORT)
            {
                HandleTransportInput(key);
            }
            else if (currentState == PlayerState.EXPLORING)
            {
                selectedNodeA?.Deselect();
                selectedNodeB?.Deselect();
                selectedNodeA = null;
                selectedNodeB = null;
            }
        }
        private void HandleModeSelection(char key)
        {
            switch (key)
            {
                case 'e':
                    currentState = PlayerState.EXPLORING;
                    break;
                case 'b':
                    currentState = PlayerState.BUILDING;
                    break;
                case 't':
                    currentState = PlayerState.MANAGING_TRANSPORT;
                    break;
                default:
                    break;
            }
        }

        private void HandleBuildingInput(char key)
        {
            buildManager.HandleKeyPressed(key, selectedNodeA);
            selectedNodeA?.Deselect();
            selectedNodeA = null;
        }
        private void HandleTransportInput(char key)
        {
            if (key == 'k')
            {
                if (selectedNodeA == null || selectedNodeB == null) return;
                TransportRoute created = transportManager.CreateRoute(selectedNodeA, selectedNodeB);
                selectedNodeA.Deselect();
                selectedNodeB.Deselect();
                selectedNodeA = null;
                selectedNodeB = null;

                if (created == null)
                {
                    Debug.Log("Route could not be created");
                    return;
                }

                SmoothLineRenderer newLine = Instantiate(smoothLineRendererPrefab, transform);
                newLine.RenderLine(created.path.ConvertAll(p => hexGrid.Grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
                activeLines.Add(newLine);
            }
            else if (key == 'r')
            {
                selectedNodeA?.Deselect();
                selectedNodeB?.Deselect();
                selectedNodeA = null;
                selectedNodeB = null;
            }
            else if (key == 'h')
            {
                foreach (var line in activeLines)
                {
                    line.HideLine();
                }
            }
            else if (key == 'v')
            {
                foreach (var line in activeLines)
                {
                    line.ShowLine();
                }
            }
        }


        private void HandleMouseClick()
        {
            WorldNode node = GetNodeUnderMouse();
            if (node == null) return;

            if (currentState == PlayerState.BUILDING)
            {
                if (selectedNodeA != null)
                {
                    selectedNodeA.Deselect();
                }
                selectedNodeA = node;
                selectedNodeA.Select();
            }
            else if (currentState == PlayerState.MANAGING_TRANSPORT)
            {
                if (selectedNodeA == null)
                {
                    selectedNodeA = node;
                    selectedNodeA.Select();
                }
                else if (selectedNodeB == null && selectedNodeA != null && !node.Position.Equals(selectedNodeA.Position))
                {
                    selectedNodeB = node;
                    selectedNodeB.Select();
                }
                else
                {
                    if (selectedNodeA.Position.Equals(node.Position))
                    {
                        selectedNodeA.Deselect();
                        selectedNodeA = null;
                    }
                    else if (selectedNodeB != null && selectedNodeB.Position.Equals(node.Position))
                    {
                        selectedNodeB.Deselect();
                        selectedNodeB = null;
                    }
                }
            }
        }


        private WorldNode GetNodeUnderMouse()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return null;
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
            return hitInfo.transform.GetComponentInParent<WorldNode>();
        }
        private enum PlayerState
        {
            EXPLORING,
            BUILDING,
            MANAGING_TRANSPORT
        }
    }
}