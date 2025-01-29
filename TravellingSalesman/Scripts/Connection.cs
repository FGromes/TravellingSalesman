using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TravellingSalesmanV2
{
    class Connection
    {
        private static HashSet<Connection> all = new HashSet<Connection>();
        public static HashSet<Connection> All { get => all; }
        string name;
        [JsonInclude]
        public string Name { get => name; private set => name = value; }
        MapPoint a;
        public MapPoint A
        {
            get => a;
            set
            {
                if (a is not null)
                {
                    a.RemoveConnection(this);
                }
                a = value;
                a.AddConnection(this);
            }
        }
        MapPoint b;
        public MapPoint B
        {
            get => b;
            set
            {
                if (b is not null)
                {
                    b.RemoveConnection(this);
                }
                b = value;
                b.AddConnection(this);
            }
        }
        public double Distance { get => GetDistance(); }

        [JsonConstructor]
        private Connection()
        {
            all.Add(this);
        }

        /// <summary>
        /// erstellt eine Verbindung von A nach B
        /// Trägt diese Connection auch direkt in den Punkten ein
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="A">Punkt A</param>
        /// <param name="B">Punkt B</param>
        /// <param name="distanz"></param>
        public Connection(string name, MapPoint A, MapPoint B) : this()
        {
            this.name = name;
            this.A = A;
            this.B = B;
        }
        private double GetDistance()
        {
            Point pointA = A.Location;
            Point pointB = B.Location;
            double distance = Math.Sqrt(Math.Pow(pointA.X - pointB.X, 2) + Math.Pow(pointA.Y - pointB.Y, 2));
            return distance;
        }

        public MapPoint? GetOtherPoint(MapPoint point)
        {
            if (point == A)
                return B;
            if (point == B)
                return A;
            return null;
        }
    }
}
