using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesman.Scripts;
using TravellingSalesmanV2;

namespace TravellingSalesman.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public RelayCommand CalculateCmd { get; set; }
        public RelayCommand SaveMapCmd { get; set; }
        public RelayCommand AddNewPoint { get; set; }

        Map Map { get; set; }

        public List<MapPoint> MapPoints 
        { 
            get => Map.Points;
        }
        public MapPoint? Start { get; set; }
        public MapPoint? End { get; set; }

        public MapPoint? SelectedPoint { get; set; }


        public MainViewModel()
        {
            CalculateCmd = new RelayCommand(o => { OnClickCalculate(); });
            SaveMapCmd = new RelayCommand(o => { OnClickSaveMap(); });
            AddNewPoint = new RelayCommand(o => { OnClickNewPoint(); });

            Map = new Map();
            Map.LoadMap();
            Start = Map.Points.Where(p => p.Name == "I").First();
            End = Map.Points.Where(p => p.Name == "O").First();
        }

        private void OnClickNewPoint()
        {
            Map.Points.Add(new MapPoint("new Point", new System.Drawing.Point(0, 0)));
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
        }
    }
}
