using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGridSelector : MonoBehaviour
{
    public Action<WorldNode> OnNodeSelected;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleMouseClick();
        }
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