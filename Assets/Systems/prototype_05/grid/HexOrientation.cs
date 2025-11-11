using UnityEngine;

namespace Systems.Prototype_05.Grid
{
    public class HexOrientation
    {
        // Axial-to-World transformation matrix components
        // First row: how Q and R affect world X
        public float QToWorldX { get; }
        public float RToWorldX { get; }

        // Second row: how Q and R affect world Z
        public float QToWorldZ { get; }
        public float RToWorldZ { get; }

        // World-to-Axial transformation matrix components (inverse matrix)
        // First row: how world X and Z affect Q
        public float WorldXToQ { get; }
        public float WorldZToQ { get; }

        // Second row: how world X and Z affect R
        public float WorldXToR { get; }
        public float WorldZToR { get; }

        public float StartAngle { get; }

        private HexOrientation(
            float qToWorldX, float rToWorldX, float qToWorldZ, float rToWorldZ,
            float worldXToQ, float worldZToQ, float worldXToR, float worldZToR,
            float startAngle)
        {
            QToWorldX = qToWorldX;
            RToWorldX = rToWorldX;
            QToWorldZ = qToWorldZ;
            RToWorldZ = rToWorldZ;
            WorldXToQ = worldXToQ;
            WorldZToQ = worldZToQ;
            WorldXToR = worldXToR;
            WorldZToR = worldZToR;
            StartAngle = startAngle;
        }

        public static readonly HexOrientation Pointy = new(
            qToWorldX: Mathf.Sqrt(3f),
            rToWorldX: Mathf.Sqrt(3f) / 2f,
            qToWorldZ: 0f,
            rToWorldZ: 3f / 2f,
            worldXToQ: Mathf.Sqrt(3f) / 3f,
            worldZToQ: -1f / 3f,
            worldXToR: 0f,
            worldZToR: 2f / 3f,
            startAngle: 0.5f
        );

        public static readonly HexOrientation Flat = new(
            qToWorldX: 3f / 2f,
            rToWorldX: 0f,
            qToWorldZ: Mathf.Sqrt(3f) / 2f,
            rToWorldZ: Mathf.Sqrt(3f),
            worldXToQ: 2f / 3f,
            worldZToQ: 0f,
            worldXToR: -1f / 3f,
            worldZToR: Mathf.Sqrt(3f) / 3f,
            startAngle: 0f
        );
    }
}