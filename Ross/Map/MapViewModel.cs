using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DLLSettingsControlPointForMap.Model;
using DtedLibrary;
using LineOfSightZoneControl;
using LineOfSightZoneControl.Models;
using Mapsui.Geometries;
using Mapsui.Styles;
using WpfMapControl;
using Point = Mapsui.Geometries.Point;
using RouteControl;
using RouteControl.Model;
using RouteControl.ViewModel;
using UIMapRast;
using Mapsui.Providers;
using Mapsui.Projection;

namespace Ross.Map
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private StatusBarModel statusBar = new StatusBarModel();
        private GridLength settingsControlWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength calculationJobHeight = new GridLength(0, GridUnitType.Pixel);


        public MapViewModel(MapControl rasterMapControl,  List<Point> polygonReturn)
        {
            this.RasterMapControl = rasterMapControl;
            InitTask1(rasterMapControl.mapControl,  polygonReturn);
            InitTask2();
            InitTask3();
        }

        public StatusBarModel StatusBar
        {
            get => statusBar;
            set
            {
                if (statusBar == value) return;
                statusBar = value;
                OnPropertyChanged();
            }
        }

        public GridLength SettingsControlWidth
        {
            get => settingsControlWidth;
            set
            {
                if(settingsControlWidth == value) return;
                settingsControlWidth = value;
                OnPropertyChanged();
            }
        }

        public GridLength CalculationJobHeight
        {
            get => calculationJobHeight;
            set
            {
                if (calculationJobHeight == value) return;
                calculationJobHeight = value;
                OnPropertyChanged();
            }
        }


        #region Task 1

        private ViewModel viewModelForTask1;
        private List<Point> polygon;
        private MapControl RasterMapControl;

        private Color orangeColor = new Color(255, 140, 0, 100);
        private Color greenColor = new Color(124, 252, 0, 100);

        public ViewModel ViewModelForTask1
        {
            get => viewModelForTask1;
            set
            {
                if (viewModelForTask1 == value) return;
                viewModelForTask1 = value;
            }
        }


        private void InitTask1(RasterMapControl rasterMapControl, List<Point> polygonReturn)
        {
            this.polygon = polygonReturn;
            viewModelForTask1 = new ViewModel(rasterMapControl);
            viewModelForTask1.PropertyChanged += ViewModelForTask1_PropertyChanged;
            viewModelForTask1.ReturnModel.PropertyChanged += ReturnModel_PropertyChanged;
        }

        private void ViewModelForTask1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(viewModelForTask1.SelectedPolygon)))
            {
               

                foreach (var item in RasterMapControl.mapControl.PolyObjects)
                {
                    if (item.Geometry is Polygon polygon)
                        if (polygon.ExteriorRing.Vertices.Count >= 360)
                        {
                            RasterMapControl.mapControl.RemoveObject(item);
                            break;
                        }
                }

                if (viewModelForTask1.SelectedPolygon.Count <= 0)
                {
                    polygon.Clear();
                    RasterMapControl.mapControl.AddPolygon(polygon, orangeColor);
                }
                else
                {
                    polygon.AddRange(viewModelForTask1.SelectedPolygon);
                    RasterMapControl.mapControl.AddPolygon(polygon, greenColor);
                }                        
            }
        }

        private void ReturnModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(viewModelForTask1.ReturnModel.Points)))
            {
                foreach (var item in RasterMapControl.mapControl.PolyObjects)
                {
                    if (item.Geometry is Polygon polygon)
                        if (polygon.ExteriorRing.Vertices.Count >= 360)
                        {
                            RasterMapControl.mapControl.RemoveObject(item);
                            break;
                        }
                }

                polygon.Clear();
                RasterMapControl.mapControl.AddPolygon(viewModelForTask1.ReturnModel.Points, orangeColor);
            }
        }

        #endregion

        #region Task 2

        private AzimuthControl.ViewModel.MainViewModel azimuth;

        public AzimuthControl.ViewModel.MainViewModel AzimuthViewModel
        {
            get => azimuth;
            set
            {
                if (azimuth == value) return;
                azimuth = value;
                OnPropertyChanged();
            }
        }

        public void InitTask2()
        {
            AzimuthViewModel = new AzimuthControl.ViewModel.MainViewModel();
            AzimuthViewModel.PropertyChanged += AzimuthViewModel_PropertyChanged;
        }

        private void AzimuthViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DrawAzimuth();
        }

        public void DrawAzimuth()
        {
            foreach(var station in AzimuthViewModel.AzimuthCollection)
            {
                List<Point> azimuthLine = new List<Point>();
                azimuthLine.Add(Mercator.FromLonLat(station.Longitude, station.Latitude));
                RasterMapControl.mapControl.AddPolyline(azimuthLine);
            }
        }

        #endregion

        #region Task 3

        private MainViewModel route;
        private Feature PolilineRoute;

        public MainViewModel RouteViewModel
        {
            get => route;
            set
            {
                if (route == value) return;
                route = value;
                OnPropertyChanged();
            }
        }

     

        private void InitTask3()
        {
            RouteViewModel = new MainViewModel();
            RouteViewModel.PropertyChanged += Route_PropertyChanged;
            RasterMapControl.OnRoutePointPosition += RasterMapControl_OnRoutePointPosition; 
        }

        private void RasterMapControl_OnRoutePointPosition(object sender, Location e)
        {
            if (RouteViewModel.SelectedItem != null)
                RouteViewModel.SelectedItem.ListPoints.Add(new WayPoints() { Latitude = e.Latitude, Longitude = e.Longitude, NumbPoint = RouteViewModel.SelectedItem.ListPoints.Count});
            else MessageBox.Show("Не выбран маршрут");
        }

        private void ListPoints_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawRoute();
        }

        private void Route_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (RouteViewModel.SelectedItem != null)
            {
                if (e.PropertyName == nameof(RouteViewModel.SelectedItem) && RouteViewModel.SelectedItem.ListPoints != null)
                {

                    RouteViewModel.SelectedItem.ListPoints.CollectionChanged -= ListPoints_CollectionChanged;
                    RouteViewModel.SelectedItem.ListPoints.CollectionChanged += ListPoints_CollectionChanged;
                    DrawRoute();
                }
            }
            else RasterMapControl.mapControl.RemoveObject(PolilineRoute);
        }

        public void DrawRoute()
        {
            List<Point> PointsOfSelectedRoute  = new List<Point>();

            RasterMapControl.mapControl.RemoveObject(PolilineRoute);

            foreach (var item in RouteViewModel.SelectedItem.ListPoints)
            {
                PointsOfSelectedRoute.Add(Mercator.FromLonLat(item.Longitude, item.Latitude));
            }

            if(RasterMapControl.mapControl.IsLoaded)
                PolilineRoute = RasterMapControl.mapControl.AddPolyline(PointsOfSelectedRoute);
        }


        #endregion

        #region OnPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
            catch { }
        }

        #endregion


    }
}
