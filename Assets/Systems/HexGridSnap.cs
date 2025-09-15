using UnityEngine;

[ExecuteAlways]
public class HexGridSnap : MonoBehaviour
{
    public GridLayout grid;          // Assign in Inspector or auto-detect

    void Reset()
    {
        if (grid == null) grid = GetComponentInParent<GridLayout>();
    }

    [ContextMenu("Snap To Hex Grid")]
    public void SnapNow()
    {
        if (grid == null) return;
        var cell = grid.WorldToCell(transform.position);               // world -> cell [22]
        Vector3 world = grid.CellToWorld(cell);                   // origin of the cell [35]
        transform.position = world;                                     // snap
    }

    void Update()
    {
        // Auto-snap while moving in the editor (optional)
        if (!Application.isPlaying) SnapNow();
    }
}
