using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DLLSettingsControlPointForMap;
using DLLSettingsControlPointForMap.Model;
using EvaTable;
using Google.Protobuf.Collections;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Styles;
using ModelsTablesDBLib;
using Ross.JSON;
using Ross.Map._EventArgs;
using RouteControl.Model;
using TransmissionPackageGroza934;
using UIMapRast.Models;
using WpfMapControl;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;
using Languages = ModelsTablesDBLib.Languages;
using LocalProperties = DLLSettingsControlPointForMap.Model.LocalProperties;
using Pen = Mapsui.Styles.Pen;
using Point = Mapsui.Geometries.Point;

namespace Ross.Map
{
    /// <summary>
    ///     Логика взаимодействия для MapLayout.xaml
    /// </summary>
    public partial class MapLayout : Window
    {
        public EventHandler<Tabl> OnPreparationMode;
        public EventHandler<Tabl> OnRadioIntelligenceMode;
        public EventHandler<Tabl> OnRadioJammingMode;
        public EventHandler<Tabl> OnPoll;
        public EventHandler<Tabl> OnTableItemDoubleClicked;

        public EventHandler<CoordEventArgs> OnCoordControlPoinChanged;
        public EventHandler<CoordEventArgs> OnCoordASPPropertyGridSelecteted;
        public EventHandler<List<Point>> OnPolygonLineOfSightChanged;
        public EventHandler<EventArgs> OnNeedToRedrawMapObjects;
        public MapViewModel mapViewModel;

        private List<Point> polygon = new List<Point>();
      
        public EventHandler<Route> RouteChanged
        {
            get => mapViewModel.OnRouteChanged;
            set => mapViewModel.OnRouteChanged = value;
        }

        public EventHandler<Route> RouteDeleted
        {
            get => mapViewModel.OnRouteDelete;
            set => mapViewModel.OnRouteDelete = value;
        }

        public EventHandler<Route> RouteClear
        {
            get => mapViewModel.OnRouteClear;
            set => mapViewModel.OnRouteClear = value;
        }

        public MapLayout()
        {
            InitializeComponent();
            Properties.OnDefaultButtonClick += Properties_OnDefaultButtonClick;
            Properties.OnApplyButtonClick += Properties_OnApplyButtonClick;
            LoadSettings();
            InitHotKeys();

            mapViewModel = new MapViewModel(RastrMap, polygon);
            DataContext = mapViewModel;


            mapObjectStyleStation = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "station.png", new Offset(0,0), scale, new Offset(0, 0));

            
        }

        #region EvaTable

        public void ClearEvaTable()
        {
            evaTable.DeleteAllItems();
        }

        public void AddStationInEvaTable(Tabl tabl)
        {           
            evaTable.AddNewItem(tabl);
        }

        public void SetStationInEvaTable(Tabl newtabl, Tabl oldTable)
        {
            evaTable.DeleteItemModel(oldTable);
            evaTable.AddNewItem(newtabl);
        }

        public Tabl GetItemFromEvaTable(int id)
        {
            return evaTable.GetModel(id);
        }
    
        public SettingsControlForMap MapProperties
        {
            get => Properties;
            set => Properties = value;
        }



        private void evaTable_ChangeFocusLine(Tabl tabl, UserControl1.StatusContextMenu menu)
        {

            OnTableItemDoubleClicked?.Invoke(this, tabl);

        }


        private void EvaTable_OnGetLine(Tabl tabl, UserControl1.StatusContextMenu menu)
        {
            switch (menu)
            {
                case UserControl1.StatusContextMenu.StatusSurvey:
                    OnPoll(this, tabl);
                    break;
                case UserControl1.StatusContextMenu.PreparationMode:
                    OnPreparationMode(this, tabl);
                    break;
                case UserControl1.StatusContextMenu.RadioIntelligenceMode:
                    OnRadioIntelligenceMode(this, tabl);
                    break;
                case UserControl1.StatusContextMenu.JammingMode:
                    OnRadioJammingMode(this, tabl);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Map

        public void TranslateMapLayout(Languages languages)
        {
            switch(languages)
            {
                case Languages.Rus:
                    ZoneControl.SetLanguage(LineOfSightZoneControl.Models.Languages.RU);
                    RouteControl.UpdateLanguage("Rus");
                    AzimuthControl.UpdateLanguage("Rus");
                    break;
                case Languages.Eng:
                    ZoneControl.SetLanguage(LineOfSightZoneControl.Models.Languages.EN);
                    RouteControl.UpdateLanguage("Eng");
                    AzimuthControl.UpdateLanguage("Eng");
                    break;
                default:
                    ZoneControl.SetLanguage(LineOfSightZoneControl.Models.Languages.RU);
                    RouteControl.UpdateLanguage("Rus");
                    AzimuthControl.UpdateLanguage("Rus");
                    break;
            }

        }

        #endregion 

        #region Hot Keys

        private void InitHotKeys()
        {
            var newCmd = new RoutedCommand();
            newCmd.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newCmd, ShowEvaTable));
        }

        private void ShowEvaTable(object sender, ExecutedRoutedEventArgs e)
        {
            if (evaTable.IsVisible)
                evaTable.Visibility = Visibility.Collapsed;
            else evaTable.Visibility = Visibility.Visible;
        }

        #endregion

        #region Properties

        private void Properties_OnApplyButtonClick(object sender, LocalProperties e)
        {
            e.Serialize("MapProperties");
        }

        private void Properties_OnDefaultButtonClick(object sender, LocalProperties e)
        {
            Properties.Local.Common.CoordinateSystem = GeographicCoordinateSystem.CK42;
            Properties.Local.Common.CoordinateView = CoordView.Dd;
        }


        private void LoadSettings()
        {
            try
            {
                MapProperties.Local = SerializerJSON.Deserialize<LocalProperties>("MapProperties");
            }
            catch
            {
            }
        }

        private void Properties_OnPathMapChanged(object sender, PathMap e)
        {

        }

        #endregion

        #region Context menu item

        private void RastrMap_OnOnPCPosition(object sender, Location e)
        {
            try
            {
                WGSCoordinate coord = new WGSCoordinate
                {
                    Latitude = Math.Round(e.Latitude, 6),
                    Longitude = Math.Round(e.Longitude, 6)
                };

                DrawRoss(coord);

                OnCoordControlPoinChanged(sender, new CoordEventArgs(new Coord() { Latitude = e.Latitude, Longitude = e.Longitude }));
            }
            catch (Exception ex)
            { 
            
            }
        }

        private void RastrMap_OnOnPointPosition(object sender, Location e)
        {
           OnCoordASPPropertyGridSelecteted(sender, new CoordEventArgs(new Coord() { Latitude = e.Latitude, Longitude = e.Longitude }));
        }

        #endregion

        #region Draw

        private const string partOfPath = @"\Resources\";
        private MapObjectStyle mapObjectStyleSqare;
        private MapObjectStyle mapObjectStyleTriangle;
        private MapObjectStyle mapObjectStyleStation;
        private double scale = 0.2;
        private float sectorAngle = 30;
        private int sectorRadius = 30000;
        private readonly Color[] colors = new Color[10]
        {
            Color.FromArgb(120, 255,102,102),
            Color.FromArgb(120, 255,178,102),
            Color.FromArgb(120, 255,255,102),
            Color.FromArgb(120, 178,255,102),
            Color.FromArgb(120, 102,255,102),
            Color.FromArgb(120, 102,255,178),
            Color.FromArgb(120, 102,255,255),
            Color.FromArgb(120, 102,178,255),
            Color.FromArgb(120, 102,102,255),
            Color.FromArgb(120, 178,102,255),
        };


        public void NavigateTo(Coord point)
        {
            var p = Mercator.FromLonLat(point.Longitude, point.Latitude);
            RastrMap.mapControl.NavigateTo(p);
        }

        public void DrawRoss(WGSCoordinate wGSCoordinate)
        {
            ControlPost controlPost = new ControlPost(0);
            controlPost.Coordinate = wGSCoordinate;

            RastrMap.UpdatePC(controlPost);
        }

        public void DrawSourceFWS(Coord point, ColorsForMap color)
        {
           var p = Mercator.FromLonLat(point.Longitude, point.Latitude);
            switch (color)
            {
                case ColorsForMap.Yellow:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "triangle_yellow.png", scale);
                    break;
                case ColorsForMap.Red:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "triangle_red.png", scale);
                    break;
                case ColorsForMap.Green:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "triangle_green.png", scale);
                    break;
                default:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "triangle_black.png", scale);
                    break;

            }

            RastrMap.mapControl.AddMapObject(mapObjectStyleTriangle, "", p);
        }


        public void DrawSourceFHSS(Coord point, ColorsForMap color)
        {
            var p = Mercator.FromLonLat(point.Longitude, point.Latitude);
            switch (color)
            {
                case ColorsForMap.Yellow:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "sqare_yellow.png", scale);
                    break;
                case ColorsForMap.Red:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "sqare_red.png", scale);
                    break;
                case ColorsForMap.Green:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "sqare_green.png", scale);
                    break;
                default:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "sqare_black.png", scale);
                    break;

            }
             
            RastrMap.mapControl.AddMapObject(mapObjectStyleSqare, "", p);
        }

        public void DrawStation(Coord point, string text = "")
        {
            var p = Mercator.FromLonLat(point.Longitude, point.Latitude);        
            RastrMap.mapControl.AddMapObject(mapObjectStyleStation, text, p);

        }

        public void ClearSectors()
        {
            RastrMap.DefinderJammingPoint.Clear();

            RastrMap.UpdateDFP(RastrMap.DefinderJammingPoint);
        }

        public void DrawSectors(Coord point, short[] lpa, int id)
        {
            DefinderJammingPoint definderJammingPoint = RastrMap.DefinderJammingPoint.Where(x=> x.ID == id).FirstOrDefault();
            if (definderJammingPoint == null)
            {
                definderJammingPoint = new DefinderJammingPoint(id);
                RastrMap.rasterViewModel.DefinderJammingPoint.Add(definderJammingPoint);
            }

            definderJammingPoint.Coordinate = new WGSCoordinate { Latitude = point.Latitude, Longitude = point.Longitude, Altitude = (float)point.Altitude};

            for (int i = 0; i < lpa.Length; i++)
            {
                var antena = new Antenna();
                antena.Sector = sectorAngle;
                antena.Radius = sectorRadius;
                antena.Direction = lpa[i];
                antena.BrushAntenna = colors[i];
                antena.PenAntenna = new Pen(colors[i], 2);
                antena.Active = true;
                RastrMap.ListRangesParam.Add(new RangesParam(10, 3000000, 0, 255, 0));


                definderJammingPoint.AntennaDefinder.Add(antena);
                definderJammingPoint.AntennaJamming.Add(antena);
            }

            RastrMap.UpdateDFP(RastrMap.rasterViewModel.DefinderJammingPoint);
        }


        public void DrawTasks()
        {
            if (ToggleButton_DownPanel.IsChecked == true)
            {
                mapViewModel.DrawAzimuth();
                mapViewModel.DrawRoute();
            }
            if (polygon.Count > 0)
                RastrMap.mapControl.AddPolygon(polygon, new Color(124, 252, 0, 100));
        }
        #endregion


        public void SetRoute(List<TableRoute> tableRoutes)
        {
            try
            {
                Route exRoute;
                foreach (var route in tableRoutes)
                {
                    exRoute = mapViewModel.RouteViewModel.RouteCollection.FirstOrDefault(t => t.Name.Equals(route.Caption));

                    if (exRoute == null)
                    {
                        var list = new ObservableCollection<WayPoints>();

                        foreach (var point in route.ListCoordinates)
                        {
                            list.Add(new WayPoints() {NumbPoint = route.ListCoordinates.IndexOf(point), Latitude = point.Coordinates.Latitude, Longitude = point.Coordinates.Longitude });
                        }

                        var newRoute = new Route();
                        newRoute.Name = route.Caption;
                        newRoute.NumbRoute = route.Id;
                        newRoute.ListPoints = list;
                        mapViewModel.RouteViewModel.RouteCollection.Add(newRoute);
                    }
                    else
                    {

                        exRoute.Name = route.Caption;
                        exRoute.NumbRoute = route.Id;
                        exRoute.ListPoints = new ObservableCollection<WayPoints>();
                        foreach (var point in route.ListCoordinates)
                        {
                            exRoute.ListPoints.Add(new WayPoints() { NumbPoint = route.ListCoordinates.IndexOf(point), Latitude = point.Coordinates.Latitude, Longitude = point.Coordinates.Longitude });
                        }
                    }

                }

                mapViewModel.RouteViewModel.UpdateRoutes();
            }catch (Exception ex)
            {

            }
        }

        public void SetASP(List<TableASP> tableASPs)
        {
            mapViewModel.ASPCollection.Clear();
            foreach (var item in tableASPs)
                mapViewModel.ASPCollection.Add(item);
        }

        public void SetCoordinateFormat(string view)
        {
            RastrMap.FormatViewCoord = (FormatCoord)ViewCoordToByte(view);
        }

        private byte ViewCoordToByte(string viewCoord)
        {
            switch (viewCoord)
            {
                case "DD.dddddd":
                    return 1;

                case "DD MM.mmmm":
                    return 2;

                case "DD MM SS.ss":
                    return 3;
            }

            return 1;
        }


        public StatusBarModel GetStatusBarModel()
        {
            return mapViewModel.StatusBar;
        }

        private void ToggleButton_DownPanel_Unchecked(object sender, RoutedEventArgs e)
        {
            OnNeedToRedrawMapObjects(this, e);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        private void ToggleButton_DownPanel_Checked(object sender, RoutedEventArgs e)
        {
            OnNeedToRedrawMapObjects(this, e);
        }
    }
}