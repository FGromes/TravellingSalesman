using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TravellingSalesman.Scripts;

namespace TravellingSalesmanV2
{
    internal class Map
    {
        protected List<MapPoint> points;
        List<List<Route>> routes;

        Dictionary<MapPoint, Route?> startPunkte;
        [JsonInclude]
        public virtual List<MapPoint> Points { get => points; protected set => points = value; }
        internal List<List<Route>> Routes { get => routes; }
        internal Dictionary<MapPoint, Route?> StartPunkte { get => startPunkte; }

        public Map()
        {
            points = new List<MapPoint>();
            routes = new List<List<Route>>();
            startPunkte = new Dictionary<MapPoint, Route?>();
        }
        public Map(List<MapPoint> points) : this()
        {
            this.points = points;
        }

        public virtual void CalculateIdealRoute(MapPoint start, MapPoint end)
        {
            startPunkte = new Dictionary<MapPoint, Route?>();
            startPunkte.Add(start, null);

            bool isDone = false;

            while (!isDone)
            {
                Dictionary<Route, double> distanzen = new Dictionary<Route, double>();
                foreach (MapPoint punkt in startPunkte.Keys)
                {
                    foreach (Connection connection in punkt.Connections)
                    {
                        MapPoint startP = punkt;
                        MapPoint? endP = connection.GetOtherPoint(punkt);
                        if (endP is null || startPunkte.ContainsKey(endP))
                            continue;

                        Route temp = new Route(startP, endP, connection);
                        temp.Before = startPunkte[startP];
                        distanzen[temp] = temp.GetTotalDistance();
                    }
                    //if (distanzen.Count > 0 && distanzen.Last().Key.End == crossings[end])
                    //{
                    //    startPunkte.Add(distanzen.Last().Key.End, distanzen.Last().Key);
                    //    break;
                    //}
                }



                if (distanzen.Count == 0)
                {
                    isDone = true;
                }
                else
                {
                    Route shortest = distanzen.OrderBy(x => x.Value).First().Key;
                    startPunkte.Add(shortest.End, shortest);
                    if (startPunkte.ContainsKey(end))
                        isDone = true;
                }
            }
        }

        public virtual void MovePoint(MapPoint point, Point newPosition)
        {
            point.MoveLocation(newPosition);
        }

        public void LoadMap()
        {
            Map map = JsonHandler.LoadMapFromJson();
            this.points = map.Points;
            this.startPunkte = map.StartPunkte;
            this.routes = map.Routes;
        }

        public async Task SaveMap()
        {
            await JsonHandler.SaveToJson(this);
        }
    }
}
