using RTSCamera;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class GridSelector : MonoBehaviour
{
    [SerializeField] private LayerMask tileLayerMask; //TODO add tile layer mask to tiles
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PathfindingController pathfindingController;

    [SerializeField] private INode selectedNode;

    private INode a;
    private RTSCameraInputs inputActions;

    public INode GetNodeUnderMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            return hexGrid.GetNode(hitInfo.transform.position);
        }
        return null;
    }

    public void Update()
    {
        INode node = GetNodeUnderMouse();

        if (node == null) return;
        if (node.Position.Equals(selectedNode?.Position)) return;

        selectedNode?.Deselect();
        node.Select();
        selectedNode = node;
    }

    void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        inputActions ??= new RTSCameraInputs();
        inputActions.Enable();

        inputActions.camera.click.performed += ctx => OnClick();
        a = null;
    }

    private void OnClick()
    {
        INode node = GetNodeUnderMouse();
        if (node == null) return;
        if (a == null)
        {
            a = node;
            a.Select();
            return;
        }
        if (a.Position.Equals(node.Position))
        {
            a = null;
            return;
        }

        Debug.Log($"Finding path from {a} to {node}");
        pathfindingController.FindPath(a, node);
        a.Deselect();
        a = null;
    }

}