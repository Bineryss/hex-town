using System.Collections.Generic;
using Systems.Core;

namespace Systems.Prototype_05.Grid
{
    public class HexGridDS
    {
        public static HexGridDS Instance => instance ??= new HexGridDS();
        private static HexGridDS instance;

        public Dictionary<AxialCoordinate, INode> grid;
    }
}