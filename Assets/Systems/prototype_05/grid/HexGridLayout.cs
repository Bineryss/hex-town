using Systems.Core;
using UnityEngine;

namespace Systems.Prototype_05.Grid
{
    public class HexGridLayout
    {
        private readonly Vector2 size;
        private readonly Vector2 origin;
        private readonly HexOrientation orientation;

        public HexGridLayout(Vector2 size, Vector2 origin = default, HexOrientation orientation = null)
        {
            this.size = size;
            this.origin = origin;
            this.orientation = orientation ?? HexOrientation.Pointy;
        }

        public Vector3 AxialToWorld(AxialCoordinate axial)
        {
            float x = (orientation.QToWorldX * axial.Q + orientation.RToWorldX * axial.R) * size.x;
            float z = (orientation.QToWorldZ * axial.Q + orientation.RToWorldZ * axial.R) * size.y;

            return new Vector3(x + origin.x, 0f, z + origin.y);
        }

        public AxialCoordinate WorldToAxial(Vector3 worldPosition)
        {
            Vector2 point = new(
                (worldPosition.x - origin.x) / size.x,
                (worldPosition.z - origin.y) / size.y
            );

            float q = orientation.WorldXToQ * point.x + orientation.WorldZToQ * point.y;
            float r = orientation.WorldXToR * point.x + orientation.WorldZToR * point.y;

            return RoundToAxial(q, r);
        }

        private AxialCoordinate RoundToAxial(float q, float r)
        {
            float s = -q - r;

            int roundQ = Mathf.RoundToInt(q);
            int roundR = Mathf.RoundToInt(r);
            int roundS = Mathf.RoundToInt(s);

            float qDiff = Mathf.Abs(roundQ - q);
            float rDiff = Mathf.Abs(roundR - r);
            float sDiff = Mathf.Abs(roundS - s);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                roundQ = -roundR - roundS;
            }
            else if (rDiff > sDiff)
            {
                roundR = -roundQ - roundS;
            }

            return new AxialCoordinate(roundQ, roundR);
        }
    }
}