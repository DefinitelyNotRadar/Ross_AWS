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
using Mapsui.Providers;
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


            mapObjectStyleStation = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "station.png", new Offset(0, 0), scale, new Offset(0, 13));

            MapProperties.Local.Common.PropertyChanged += Common_PropertyChanged;


            var sectorsSize =  InitializeSectorSizes();
            sectorsAngles = new int[] { sectorsSize.Sector_LPA_5_10, sectorsSize.Sector_LPA_BPSS, sectorsSize.Sector_LPA_1_3, sectorsSize.Sector_LPA_2_4 };
        }



        public SectorsSize InitializeSectorSizes()
        {
            SectorsSize sectorsSize = null;
            sectorsSize = SerializerJSON.Deserialize<SectorsSize>("config");
            if (sectorsSize == null)
            {
                sectorsSize = new SectorsSize();
                SerializerJSON.Serialize(sectorsSize, "config");
            }

           return sectorsSize;
        }


        private void Common_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapProperties.Local.Common.Access))
            {
                if (MapProperties.Local.Common.Access == 0)
                    ToggleButton_Setting.IsChecked = false;
            }
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
            switch (languages)
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
                var mp = SerializerJSON.Deserialize<LocalProperties>("MapProperties");
                MapProperties.Local = mp ?? new LocalProperties();
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
        private double scale = 0.21;
        private double iriscale = 0.29;
        private int sectorRadius = 30000;
        private readonly Color[] colors = new Color[]
        {
            Color.FromArgb(100, 162,58,255),
            Color.FromArgb(100, 255,18,18),
            Color.FromArgb(100, 255,97,0),
            Color.FromArgb(100, 230,255,0),
        };
        private readonly int[] sectorsAngles;


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

        public void DrawSourceFWS(Coord point, ColorsForMap color, double? freq)
        {
           var p = Mercator.FromLonLat(point.Longitude, point.Latitude);
            switch (color)
            {
                case ColorsForMap.Yellow:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "IRIFRCHT.png", new Offset(0,0), iriscale, new Offset(0,15));
                    break;
                case ColorsForMap.Red:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "IRIFRCHJam.png", new Offset(0, 0), iriscale, new Offset(0, 15));
                    break;
                case ColorsForMap.Green:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "IRIFRCH.png", new Offset(0, 0), iriscale, new Offset(0, 15));
                    break;
                default:
                    mapObjectStyleTriangle = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "IRIFRCH.png", new Offset(0, 0), iriscale, new Offset(0, 15));
                    break;

            }

            RastrMap.mapControl.AddMapObject(mapObjectStyleTriangle, freq != null ? Math.Round((double)freq) +" kHz" : "", p);
        }


        public void DrawSourceFHSS(Coord point, ColorsForMap color)
        {
            var p = Mercator.FromLonLat(point.Longitude, point.Latitude);
            switch (color)
            {
                case ColorsForMap.Yellow:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "PPRCHT.png", iriscale);
                    break;
                case ColorsForMap.Red:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "PPRCHJam.png", iriscale);
                    break;
                case ColorsForMap.Green:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "PPRCH.png", iriscale);
                    break;
                default:
                    mapObjectStyleSqare = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "PPRCH.png", iriscale);
                    break;

            }
             
            RastrMap.mapControl.AddMapObject(mapObjectStyleSqare, "", p);
        }

        public void DrawStation(Coord point, string text = "")
        {
            var p = Mercator.FromLonLat(point.Longitude, point.Latitude);        
            RastrMap.mapControl.AddMapObject(mapObjectStyleStation, text, p);

        }

        List<IFeature> drawedPelengs = new List<IFeature>();
        List<IMapObject> drawedTextPelengs = new List<IMapObject>();
        MapObjectStyle pelObjStyle = new MapObjectStyle(new SymbolStyle() { SymbolType = SymbolType.Ellipse,SymbolScale = 0.71 }, new Offset(0,-7));
        public void DrawPeleng(Coord startPoint, float angle, float distance)
        {
            try
            {
               

                var p = Mercator.FromLonLat(startPoint.Longitude, startPoint.Latitude);
                var secondPoint = GetCirclePoint(p, angle, distance * 2.2);
                var halfPoint = GetCirclePoint(p, angle, distance);



                var pList = new List<Point>() { p, secondPoint };
                var ray = RastrMap.mapControl.AddPolyline(pList);
                var text = RastrMap.mapControl.AddMapObject(pelObjStyle, Math.Round(angle).ToString() + "°", halfPoint);

                drawedTextPelengs.Add(text);
                drawedPelengs.Add(ray);
            }
            catch { }         
        }

        private Mapsui.Geometries.Point GetCirclePoint(Mapsui.Geometries.Point centerPoint, float angle, double size)
        {
            double num = (double)(90f - angle) * Math.PI / 180.0;
            double x = centerPoint.X + Math.Cos(num) * size;
            double y = centerPoint.Y + Math.Sin(num) * size;
            return new Mapsui.Geometries.Point(x, y);
        }

        public void RemovePelengs()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var p in drawedPelengs)
                {
                    RastrMap.mapControl.RemoveObject(p);
                }
                drawedPelengs.Clear();

                foreach (var p in drawedTextPelengs)
                {
                    RastrMap.mapControl.RemoveObject(p);
                }
                drawedTextPelengs.Clear();
            });
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

            bool[] sectorShow = new bool[] { mapViewModel.ShowSectorsModel.IsShowLPA510Sector,
                                             mapViewModel.ShowSectorsModel.IsShowLBPSSSector,
                                             mapViewModel.ShowSectorsModel.IsShowLPA13Sector,
                                             mapViewModel.ShowSectorsModel.IsShowLPA24Sector};

            for (int i = 0; i < lpa.Length; i++)
            {
                if (sectorShow[i])
                {
                    var antena = new Antenna();
                    antena.Sector = sectorsAngles[i];
                    antena.Radius = sectorRadius;
                    antena.Direction = lpa[i];
                    antena.BrushAntenna = colors[i];
                    antena.PenAntenna = new Pen(colors[i], 2);
                    antena.Active = true;
                    RastrMap.ListRangesParam.Add(new RangesParam(10, 3000000, 0, 255, 0));


                    definderJammingPoint.AntennaDefinder.Add(antena);
                    definderJammingPoint.AntennaJamming.Add(antena);
                }
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

        #region Set some settings
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
        #endregion


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