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
using ModelsTablesDBLib;
using System.Collections.ObjectModel;

namespace Ross.Map
{
    public class MapViewModel : INotifyPropertyChanged
    {
        private StatusBarModel statusBar = new StatusBarModel();
        private GridLength settingsControlWidth = new GridLength(0, GridUnitType.Pixel);
        private GridLength calculationJobHeight = new GridLength(0, GridUnitType.Pixel);
        private ObservableCollection<TableASP> tableASPs = new ObservableCollection<TableASP>();


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
            //set
            //{
            //    if (statusBar == value) return;
            //    statusBar = value;
            //    OnPropertyChanged();
            //}
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

        public ObservableCollection<TableASP> ASPCollection
        {
            get => tableASPs;
            
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
            tableASPs.CollectionChanged += TableASPs_CollectionChanged1;
        }

        private void TableASPs_CollectionChanged1(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //viewModelForTask1.LineOfSightModel.
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
            tableASPs.CollectionChanged += TableASPs_CollectionChanged;
        }

        private void TableASPs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AzimuthViewModel.AzimuthCollection.Clear();
            foreach (var asp in ASPCollection)
                AzimuthViewModel.AzimuthCollection.Add(new AzimuthControl.Model.Azimuth() { Latitude = asp.Coordinates.Latitude, Longitude = asp.Coordinates.Longitude, NumJammer = asp.Id });
        }

        private void AzimuthViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DrawAzimuth();
        }

        public void DrawAzimuth()
        {
            
            foreach (var azimuth in AzimuthViewModel.AzimuthCollection)
            {
                List<Point> azimuthLine = new List<Point>();
                azimuthLine.Add(Mercator.FromLonLat(86.448, 175.34585));
                azimuthLine.Add(Mercator.FromLonLat(AzimuthViewModel.UserLongitude, AzimuthViewModel.UserLatitude));
                azimuthLine.Add(Mercator.FromLonLat(azimuth.Longitude, azimuth.Latitude));

                RasterMapControl.mapControl.AddPolyline(azimuthLine, Color.Green);
            }

           
        }

        #endregion

        #region Task 3

        private MainViewModel route;
        private Feature PolilineRoute;
        private List<IMapObject> FlagsObjects;
        private IMapObject FinishFlagObject;

        private MapObjectStyle mapObjectStyle_Part;
        private MapObjectStyle mapObjectStyle_Finish;
        private const string partOfPath = @"\Resources\";

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
            mapObjectStyle_Part = RasterMapControl.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "StartPoint.png", 0.27);
            mapObjectStyle_Finish = RasterMapControl.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "StopPoint.png", 0.07);
        }

        private void RasterMapControl_OnRoutePointPosition(object sender, Location e)
        {
            if (RouteViewModel.SelectedItem != null)
            { 
                RouteViewModel.SelectedItem.ListPoints.Add(new WayPoints() { Latitude = e.Latitude, Longitude = e.Longitude, NumbPoint = RouteViewModel.SelectedItem.ListPoints.Count });
                RouteViewModel.SelectedItem = RouteViewModel.SelectedItem;
            }

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
            if(FlagsObjects != null)
                foreach(var flag in FlagsObjects)
                    RasterMapControl.mapControl.RemoveObject(flag);
            RasterMapControl.mapControl.RemoveObject(PolilineRoute);
            RasterMapControl.mapControl.RemoveObject(FinishFlagObject);

            List<Point> PointsOfSelectedRoute = new List<Point>();
            FlagsObjects = new List<IMapObject>();

            foreach (var item in RouteViewModel.SelectedItem.ListPoints)
            {
                var p = Mercator.FromLonLat(item.Longitude, item.Latitude);
                
                PointsOfSelectedRoute.Add(p);
               FlagsObjects.Add(RasterMapControl.mapControl.AddMapObject(mapObjectStyle_Part,"",p));
            }

            if (FlagsObjects != null && FlagsObjects.Count > 0)
                RasterMapControl.mapControl.RemoveObject(FlagsObjects.Last());
            if(PointsOfSelectedRoute.Count > 0)
                FinishFlagObject = RasterMapControl.mapControl.AddMapObject(mapObjectStyle_Finish, "", PointsOfSelectedRoute.Last());

            if (RasterMapControl.mapControl.IsLoaded)
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
