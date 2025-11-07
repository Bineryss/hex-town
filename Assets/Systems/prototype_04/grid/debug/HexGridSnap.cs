using UnityEngine;

[ExecuteAlways]
public class HexGridSnap : MonoBehaviour
{
    public GridLayout grid;          

    void Reset()
    {
        if (grid == null) grid = GetComponentInParent<GridLayout>();
    }

    [ContextMenu("Snap To Hex Grid")]
    public void SnapNow()
    {
        if (grid == null) return;
        var cell = grid.WorldToCell(transform.position);
        Vector3 world = grid.CellToWorld(cell);
        transform.position = world;
    }

    void Update()
    {
        if (!Application.isPlaying) SnapNow();
    }
}
