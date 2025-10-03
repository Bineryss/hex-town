using System;
using System.Collections.Generic;
using RTSCamera;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(PlayerInput))]

public class BuildManager : SerializedMonoBehaviour
{
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private PlayerInput playerInput;
    [OdinSerialize] private Dictionary<char, WorldTile> buildables;

    [Header("Debug Info")]
    [SerializeField] private WorldNode selected;

    private RTSCameraInputs inputActions;
    void Awake()
    {
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }
        inputActions ??= new RTSCameraInputs();
        inputActions.Enable();

        inputActions.camera.click.performed += ctx => OnClick(ctx);
        Keyboard.current.onTextInput += ctx => HandleKeyPressed(ctx);
    }

    private void HandleKeyPressed(char ctx)
    {
        if (selected == null) return;
        if (!buildables.ContainsKey(ctx)) return;
        if (!selected.worldTile.isBuildable) return;

        if (buildables.TryGetValue(ctx, out WorldTile tile))
        {
            selected.worldTile = tile;
            selected.Deselect();
            selected = null;
        }
    }

    private void GetNodeUnderMouse()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hitInfo)) return;
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);
        WorldNode node = hitInfo.transform.GetComponentInParent<WorldNode>();

        if (node == null) return;

        if (selected == null)
        {
            node.Select();
            selected = node;
            return;
        }

        if (node.Position.Equals(selected.Position))
        {
            selected.Deselect();
            selected = null;
        }
        else
        {
            selected.Deselect();
            node.Select();
            selected = node;
        }
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;

        GetNodeUnderMouse();
    }
}