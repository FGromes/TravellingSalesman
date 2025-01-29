using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanV2
{
    internal class Route
    {
        public MapPoint Start { get; set; }
        public MapPoint End { get; set; }
        public Connection Connection { get; set; }
        public Route Before { get; set; }


        public Route(MapPoint Start, MapPoint End, Connection Connection)
        {
            this.Start = Start;
            this.End = End;
            this.Connection = Connection;
        }
        public Route(MapPoint Start, MapPoint End, Connection Connection, Route Before) : this(Start, End, Connection)
        {
            this.Before = Before;
        }

        public double GetTotalDistance()
        {
            double totalDistance = Connection.Distance;
            if (Before != null)
                totalDistance += Before.GetTotalDistance();
            return totalDistance;
        }

        public List<Route> GetRoutes()
        {
            if (Before is null)
                return new List<Route>() { this };
            return new List<Route>(Before.GetRoutes()) { this };
        }
    }
}
