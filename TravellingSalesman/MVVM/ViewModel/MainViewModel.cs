using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using TravellingSalesman.Scripts;
using TravellingSalesmanV2;

namespace TravellingSalesman.MVVM.ViewModel
{
    internal class MainViewModel: ObservableObject
    {
        public RelayCommand CalculateCmd { get; private set; }
        public RelayCommand SaveMapCmd { get; private set; }
        public RelayCommand LoadDefaultMapCmd { get; private set; }
        public RelayCommand AddNewPointCmd { get; private set; }
        public RelayCommand ConnectPointsCmd { get; private set; }

        Map Map { get; set; }

        public ObservableCollection<MapPoint> MapPoints 
        { 
            get => Map.Points;
        }
        public MapPoint? Start { get; set; }
        public MapPoint? End { get; set; }

        //public MapPoint? SelectedPoint { get; set; }

        Image mapImage;
        public Image MapImage 
        { 
            get => mapImage;
            set
            {
                mapImage = value;
                OnPropertyChanged();
            }
        }


        int circleRadius = 25;

        public MainViewModel()
        {
            CalculateCmd = new RelayCommand(o => { OnClickCalculate(); });
            SaveMapCmd = new RelayCommand(o => { OnClickSaveMap(); });
            LoadDefaultMapCmd = new RelayCommand(o => { OnClickLoadDefaultMap(); });
            AddNewPointCmd = new RelayCommand(o => { OnClickNewPoint(); });
            ConnectPointsCmd = new RelayCommand(o => { OnConnectMapPoints(); });

            Map = new Map();
            Map.LoadMap();
            Start = Map.Points.Where(p => p.Name == "I").First();
            End = Map.Points.Where(p => p.Name == "O").First();

            MapImage = new Image();
            DrawMapImage();
        }

        private void OnClickLoadDefaultMap()
        {
            List<MapPoint> defaultPoints = MapPoint.GetDefaultMapPoints();
            Map = new Map(defaultPoints);
            DrawMapImage();
        }

        private void OnConnectMapPoints()
        {
            List<MapPoint> selectedPoints = Map.Points.Where(point => point.IsSelected).ToList();
            Debug.Print($"Selected Points: {selectedPoints.Count}");
            foreach (MapPoint point in selectedPoints)
            {
                foreach(MapPoint target in selectedPoints)
                {
                    if (point == target) continue;
                    if (point.Connections.Count == 0 || point.Connections.Where(c => c.GetOtherPoint(point) == target).ToList().Count == 0)
                    {
                        new Connection("", point, target);
                    }
                }
            }
            DrawMapImage();
        }

        public void OnImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            // get mouse position to match a MapPoint
            Point position = e.GetPosition((Image)sender);
            MapPoint? selectedPoint = null;
            Debug.Print($"e.Pos: {position}");

            foreach(MapPoint point in Map.Points)
            {
                if(Math.Pow(position.X - point.Location.X, 2) +
                    Math.Pow(position.Y - point.Location.Y, 2) <= Math.Pow(circleRadius, 2))
                {
                    Debug.Print($"Point: {point.Name} Pos: {point.Location}");
                    selectedPoint = point;
                    break;
                }
            }
            //start dragdrop

            // Move the dragged node when the left mouse button is used.
            if (e.ChangedButton == MouseButton.Left && selectedPoint is not null)
            {
                DragDrop.DoDragDrop(sender as Image, selectedPoint, DragDropEffects.Move);
            }
        }

        public void OnDropMapPoint(object sender, DragEventArgs e)
        {
            // Retrieve the client coordinates of the drop location.
            Point targetPosition = e.GetPosition((Image)sender);

            if (e.Data.GetDataPresent(typeof(MapPoint)))
            {
                MapPoint droppedMapPoint = (MapPoint)e.Data.GetData(typeof(MapPoint));

                if (droppedMapPoint is not null)
                    MovePoint(droppedMapPoint, targetPosition);
            }
        }

        private void MovePoint(MapPoint droppedPoint, Point targetPoint)
        {
            Map.MovePoint(droppedPoint, targetPoint);
            DrawMapImage();
        }

        private void DrawMapImage()
        {
            // to minimize overdraw keeping Track of already drawn points and connections
            List<MapPoint> donePoints = new List<MapPoint>();
            List<Connection> doneConnections = new List<Connection>();


            DrawingGroup mapDrawings = new DrawingGroup();


            // Draw all Connections first to have Points in the foreground
            // Starting with Routes Start to End smallest subset first

            // (Green Group)
            if (End is not null && Map.StartPunkte.ContainsKey(End))
            {
                List<Route> routes = Map.StartPunkte[End].GetRoutes();
                foreach (Route line in routes)
                {
                    if (doneConnections.Contains(line.Connection))
                        continue;

                    GeometryDrawing routeDrawing = new GeometryDrawing();
                    routeDrawing.Brush = Brushes.Green;
                    routeDrawing.Pen = new Pen(Brushes.Green, 3);
                    routeDrawing.Geometry = new LineGeometry(line.Start.Location, line.End.Location);

                    mapDrawings.Children.Add(routeDrawing);
                    doneConnections.Add(line.Connection);
                }
            }

            // (Red Group)
            foreach (Route route in Map.StartPunkte.Values)
            {
                if (route is null)
                    continue;

                foreach (Route line in route.GetRoutes())
                {
                    if (doneConnections.Contains(line.Connection))
                        continue;

                    GeometryDrawing routeDrawing = new GeometryDrawing();
                    routeDrawing.Brush = Brushes.Red;
                    routeDrawing.Pen = new Pen(Brushes.Red, 3);
                    routeDrawing.Geometry = new LineGeometry(line.Start.Location, line.End.Location);

                    mapDrawings.Children.Add(routeDrawing);
                    doneConnections.Add(line.Connection);
                }
            }

            // (Black Group)
            foreach (Connection connection in Connection.All)
            {
                if (doneConnections.Contains(connection)) 
                    continue;

                GeometryDrawing routeDrawing = new GeometryDrawing();
                routeDrawing.Brush = Brushes.Black;
                routeDrawing.Pen = new Pen(Brushes.Black, 1);
                routeDrawing.Geometry = new LineGeometry(connection.A.Location, connection.B.Location);

                mapDrawings.Children.Add(routeDrawing);
                doneConnections.Add(connection);
            }
            foreach (Connection connection in Connection.All)
            {
                FormattedText formatted = new FormattedText(Math.Round(connection.Distance).ToString(),
                                                                CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight,
                                                                new Typeface("Verdana"),
                                                                18,
                                                                Brushes.Black,
                                                                1f);

                GeometryDrawing textDrawing = new GeometryDrawing();
                textDrawing.Brush = Brushes.Black;
                //textDrawing.Pen = new Pen(Brushes.Black, 1);
                textDrawing.Geometry = formatted.BuildGeometry(
                    new Point((connection.A.Location.X + connection.B.Location.X) / 2,
                              (connection.A.Location.Y + connection.B.Location.Y) / 2));

                mapDrawings.Children.Add(textDrawing);
            }


            // Points
            // Start and End Points first
            if (Start is not null)
            {
                GeometryDrawing startPointDrawing = new GeometryDrawing();
                startPointDrawing.Brush = Brushes.Blue;
                //startPointDrawing.Pen = new Pen();
                startPointDrawing.Geometry = new EllipseGeometry(Start.Location, circleRadius, circleRadius);

                mapDrawings.Children.Add(startPointDrawing);
                donePoints.Add(Start);
            }
            if (End is not null)
            {
                GeometryDrawing endPointDrawing = new GeometryDrawing();
                endPointDrawing.Brush = Brushes.Blue;
                //endPointDrawing.Pen = new Pen(Brushes.Blue, 3);
                endPointDrawing.Geometry = new EllipseGeometry(End.Location, circleRadius, circleRadius);

                mapDrawings.Children.Add(endPointDrawing);
                donePoints.Add(End);

                // Points on shortest Path
                // Green
                if (Map.StartPunkte.ContainsKey(End))
                {
                    List<Route> routes = Map.StartPunkte[End].GetRoutes();
                    foreach (Route line in routes)
                    {
                        if(donePoints.Contains(line.End))
                            continue;
                        GeometryDrawing routePointDrawing = new GeometryDrawing();
                        routePointDrawing.Brush = Brushes.Green;
                        //routePointDrawing.Pen = new Pen(Brushes.Green, 3);
                        routePointDrawing.Geometry = new EllipseGeometry(line.End.Location, circleRadius, circleRadius);

                        mapDrawings.Children.Add(routePointDrawing);
                        donePoints.Add(line.End);
                    }
                }
            }
            // failed Paths
            // Red
            foreach (Route route in Map.StartPunkte.Values)
            {
                if (route is null)
                    continue;

                foreach (Route line in route.GetRoutes())
                {
                    if (donePoints.Contains(line.End))
                        continue;

                    GeometryDrawing routePointDrawing = new GeometryDrawing();
                    routePointDrawing.Brush = Brushes.Red;
                    //routePointDrawing.Pen = new Pen(Brushes.Red, 3);
                    routePointDrawing.Geometry = new EllipseGeometry(line.End.Location, circleRadius, circleRadius);

                    mapDrawings.Children.Add(routePointDrawing);
                    donePoints.Add(line.End);
                }
            }


            // left over Points
            // Black
            foreach (MapPoint point in Map.Points)
            {
                if (donePoints.Contains(point))
                    continue;

                GeometryDrawing routePointDrawing = new GeometryDrawing();
                routePointDrawing.Brush = Brushes.Black;
                //routePointDrawing.Pen = new Pen(Brushes.Black, 1);
                routePointDrawing.Geometry = new EllipseGeometry(point.Location, circleRadius, circleRadius);
                mapDrawings.Children.Add(routePointDrawing);
                donePoints.Add(point);
            }

            foreach (MapPoint point in Map.Points)
            {
                FormattedText formatted = new FormattedText(point.Name,
                                                                CultureInfo.GetCultureInfo("en-us"),
                                                                FlowDirection.LeftToRight,
                                                                new Typeface("Verdana"),
                                                                23,
                                                                Brushes.White,
                                                                1f);

                GeometryDrawing textDrawing = new GeometryDrawing();
                textDrawing.Brush = Brushes.White;
                textDrawing.Pen = new Pen(Brushes.White, 1);
                textDrawing.Geometry = formatted.BuildGeometry(new Point(point.Location.X - 7, point.Location.Y - circleRadius / 2));

                mapDrawings.Children.Add(textDrawing);
            }
            GeometryDrawing test = new GeometryDrawing();
            test.Brush = Brushes.White;
            test.Geometry = new EllipseGeometry(new Point(0, 0), 1, 1);
            mapDrawings.Children.Add(test);

            DrawingImage drawingMapSource = new DrawingImage(mapDrawings);
            drawingMapSource.Freeze();

            MapImage = new Image();
            MapImage.Stretch = Stretch.None;
            MapImage.Source = drawingMapSource;
        }
        private void OnDropPoint(object sender, DragEventArgs e)
        {
            Debug.Print("OnDropPoint");
        }

        private void OnClickNewPoint()
        {
            Map.Points.Add(new MapPoint("new Point", new Point(circleRadius, circleRadius)));
            DrawMapImage();
        }

        private async void OnClickSaveMap()
        {
            await Map.SaveMap();
        }

        private void OnClickCalculate()
        {
            if (Start is not null && End is not null 
                && Start != End)
            {
                Map.CalculateIdealRoute(Start, End);
            }
            DrawMapImage();
        }
    }
}
