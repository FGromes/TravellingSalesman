using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TravellingSalesman.Scripts;

namespace TravellingSalesmanV2
{
    class MapPoint
    {
        [JsonInclude]
        public string Name { get; private set; }
        [JsonInclude]
        public Point Location { get; private set; }
        [JsonInclude]
        public List<Connection> Connections { get; private set; }

        [JsonConstructor]
        private MapPoint()
        {
            Connections = new List<Connection>();
        }
        public MapPoint(string name, Point location) : this()
        {
            this.Name = name;
            this.Location = location;
        }

        public MapPoint(string name, Point location, List<Connection> connections) : this(name, location)
        {
            this.Connections = connections;
        }

        public void MoveLocation(Point location)
        {
            this.Location = location;
        }

        public void AddConnection(Connection connection)
        {
            if (!Connections.Contains(connection))
                Connections.Add(connection);
        }

        public void RemoveConnection(Connection connection)
        {
            if (Connections.Contains(connection))
                Connections.Remove(connection);
        }


        public static List<MapPoint> GetDefaultMapPoints()
        {
            List<MapPoint> mapPoints = new List<MapPoint>();

            MapPoint A = new MapPoint("A", new Point(200, 30));
            mapPoints.Add(A);
            MapPoint B = new MapPoint("B", new Point(250, 225));
            mapPoints.Add(B);
            MapPoint C = new MapPoint("C", new Point(100, 150));
            mapPoints.Add(C);
            MapPoint D = new MapPoint("D", new Point(400, 75));
            mapPoints.Add(D);
            MapPoint E = new MapPoint("E", new Point(468, 590));
            mapPoints.Add(E);
            MapPoint F = new MapPoint("F", new Point(267, 573));
            mapPoints.Add(F);
            MapPoint G = new MapPoint("G", new Point(503, 403));
            mapPoints.Add(G);
            MapPoint H = new MapPoint("H", new Point(263, 319));
            mapPoints.Add(H);
            MapPoint I = new MapPoint("I", new Point(50, 250));
            mapPoints.Add(I);
            MapPoint K = new MapPoint("K", new Point(360, 450));
            mapPoints.Add(K);
            MapPoint L = new MapPoint("L", new Point(532, 202));
            mapPoints.Add(L);
            MapPoint M = new MapPoint("M", new Point(50, 50));
            mapPoints.Add(M);
            MapPoint N = new MapPoint("N", new Point(300, 115));
            mapPoints.Add(N);
            MapPoint O = new MapPoint("O", new Point(169, 687));
            mapPoints.Add(O);
            MapPoint P = new MapPoint("P", new Point(207, 393));
            mapPoints.Add(P);
            MapPoint Q = new MapPoint("Q", new Point(83, 539));
            mapPoints.Add(Q);
            MapPoint X = new MapPoint("X", new Point(195, 138));
            mapPoints.Add(X);
            MapPoint Y = new MapPoint("Y", new Point(366, 344));
            mapPoints.Add(Y);
            MapPoint Z = new MapPoint("Z", new Point(385, 221));
            mapPoints.Add(Z);



            new Connection("", M, A);
            new Connection("", M, X);
            new Connection("", M, C);
            new Connection("", M, I);

            new Connection("", A, D);
            new Connection("", A, N);
            new Connection("", A, B);

            new Connection("", X, N);
            new Connection("", X, C);
            new Connection("", X, B);

            new Connection("", C, I);

            new Connection("", I, B);
            new Connection("", I, H);
            new Connection("", I, P);

            new Connection("", B, D);
            new Connection("", B, Z);
            new Connection("", B, H);

            new Connection("", N, D);
            new Connection("", N, Z);

            new Connection("", D, L);

            new Connection("", Z, L);
            new Connection("", Z, Y);

            new Connection("", L, H);
            new Connection("", L, G);

            new Connection("", H, Y);
            new Connection("", H, P);
            new Connection("", H, K);

            new Connection("", Y, K);
            new Connection("", Y, G);

            new Connection("", P, K);
            new Connection("", P, F);
            new Connection("", P, O);

            new Connection("", G, K);
            new Connection("", G, E);

            new Connection("", K, E);
            new Connection("", K, F);

            new Connection("", E, F);
            new Connection("", E, O);

            new Connection("", F, O);

            new Connection("", O, Q);

            return mapPoints;
        }

    }
}
