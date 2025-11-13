using System;
using System.Collections.Generic;

namespace Systems.Prototype_05.Transport
{
    public class TradeRouteDS
    {
        public static TradeRouteDS Instance => instance ??= new TradeRouteDS();
        private static TradeRouteDS instance;
        public Dictionary<Guid, TransportRoute> routes = new();
    }
}