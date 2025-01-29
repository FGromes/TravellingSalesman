using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TravellingSalesmanV2;

namespace TravellingSalesman.Scripts
{
    internal class ObservableMap : Map, INotifyPropertyChanged
    {
        ObservableCollection<MapPoint> mapPoints;
        public ObservableCollection<MapPoint> MapPoints
        {
            get => mapPoints;
            set
            {
                mapPoints = value;
                OnPropertyChanged();
            }
        }

        public override List<MapPoint> Points { get => points; protected set { points = value; } }

        public ObservableMap() : base()
        {
            MapPoints = new ObservableCollection<MapPoint>();
            //MapPoints.CollectionChanged
        }

        public override void CalculateIdealRoute(MapPoint start, MapPoint end)
        {
            base.CalculateIdealRoute(start, end);
        }

        public override void MovePoint(MapPoint point, Point newPosition)
        {
            base.MovePoint(point, newPosition);
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        void CollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged();
        }
        protected void CollectionPropertyChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                foreach (var item in e.NewItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(CollectionItemPropertyChanged);
                }
            }

            if (e.OldItems is not null)
            {
                foreach (var item in e.OldItems)
                {
                    (item as INotifyPropertyChanged).PropertyChanged -= new PropertyChangedEventHandler(CollectionItemPropertyChanged);
                }
            }
        }
    } 
}
