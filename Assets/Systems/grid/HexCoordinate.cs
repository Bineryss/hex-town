using Sirenix.OdinInspector;
using UnityEngine;

namespace Systems.Grid
{
    [System.Serializable]
    [InlineProperty]
    public struct HexCoordinate
    {

        [SerializeField, LabelWidth(13), HorizontalGroup("Cube", 0.33f)]
        private int q;
        [SerializeField, LabelWidth(13), HorizontalGroup("Cube", 0.33f)]
        private int r;

        public readonly int Q => q;
        public readonly int R => r;
        [ShowInInspector, LabelWidth(13), HorizontalGroup("Cube")]
        public readonly int S => -Q - R;

        public HexCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public static HexCoordinate operator -(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a.Q - b.Q, a.R - b.R);
        }
        public static HexCoordinate operator +(HexCoordinate a, HexCoordinate b)
        {
            return new HexCoordinate(a.Q + b.Q, a.R + b.R);
        }
        public static HexCoordinate FromOffsetCoordinates(int col, int row)
        {
            int q = col - ((row - (row & 1)) / 2);
            return new HexCoordinate(q, row);
        }
        public Vector3Int ToOffset()
        {
            int col = Q + (R - (R & 1)) / 2;
            int row = R;
            return new Vector3Int(col, row, 0);
        }
        public int Distance(HexCoordinate other)
        {
            HexCoordinate vector = this - other;
            return (
                (Mathf.Abs(vector.Q) + Mathf.Abs(vector.R) + Mathf.Abs(vector.S)) / 2
            );
        }
        public override string ToString()
        {
            return $"(Q {Q},R {R},S {S})";
        }

        public string ToStringOnSeparateLines()
        {
            return $"{Q}\n{S}\n{R}";
        }

        public override bool Equals(object obj)
        {
            if (obj is not HexCoordinate) return false;
            var hc = (HexCoordinate)obj;
            return q == hc.q && r == hc.r;
        }
        public override int GetHashCode()
        {
            return q ^ r << 2;
        }
    }
}