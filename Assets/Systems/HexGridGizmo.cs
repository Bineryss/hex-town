using UnityEngine;

[ExecuteAlways]
public class HexGridGizmo : MonoBehaviour
{
    public GridLayout grid;          // Assign your Hex Grid
    public int radiusInCells = 6;    // How far to draw from 0,0
    public bool drawCenters = true;
    public bool drawOutlines = true;
    public bool flatTop = false;     // false = point-top, true = flat-top
    public float visualRadius = 0.5f; // world units (outer radius)

    void OnDrawGizmos()
    {
        if (grid == null) return;

        Gizmos.color = new Color(0f, 1f, 1f, 0.8f);

        for (int y = -radiusInCells; y <= radiusInCells; y++)
        {
            for (int x = -radiusInCells; x <= radiusInCells; x++)
            {
                var cell = new Vector3Int(x, y, 0);
                Vector3 center = grid.CellToWorld(cell);

                if (drawCenters)
                {
                    Gizmos.DrawSphere(center, 0.05f);
                }

                if (drawOutlines)
                {
                    DrawHex(center, visualRadius, flatTop);
                }
            }
        }
    }

    // Draws a hex on the XZ plane.
    void DrawHex(Vector3 center, float r, bool flat)
    {
        // For point-top, start at 30°; for flat-top, start at 0° (Red Blob convention)
        float startDeg = flat ? 0f : 30f;
        Vector3 prev = HexCorner(center, r, startDeg, flat);
        for (int i = 1; i <= 6; i++)
        {
            Vector3 next = HexCorner(center, r, startDeg + 60f * i, flat);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }

    Vector3 HexCorner(Vector3 c, float r, float deg, bool flat)
    {
        float rad = deg * Mathf.Deg2Rad;
        // Draw on XZ plane (Y up)
        float x = c.x + r * Mathf.Cos(rad);
        float z = c.z + r * Mathf.Sin(rad);
        return new Vector3(x, c.y, z);
    }
}
