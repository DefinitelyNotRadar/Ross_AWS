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

        public EventHandler<CoordEventArgs> OnCoordControlPoinChanged;
        public EventHandler<CoordEventArgs> OnCoordASPPropertyGridSelecteted;
        public EventHandler<List<Point>> OnPolygonLineOfSightChanged;
        public EventHandler<EventArgs> OnNeedToRedrawMapJojects;


        private List<Point> polygon = new List<Point>();

        public MapLayout()
        {
            InitializeComponent();
            Properties.OnDefaultButtonClick += Properties_OnDefaultButtonClick;
            Properties.OnApplyButtonClick += Properties_OnApplyButtonClick;
            LoadSettings();
            InitHotKeys();

            DataContext = new MapViewModel(RastrMap, polygon);


            mapObjectStyleStation = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "station.png", new Offset(0,-130), scale, new Offset(0, 0));

            
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
            Properties.Local.ColorsMap.ColorIRIFUSS = ColorsForMap.Yellow;
            Properties.Local.ColorsMap.ColorIRIFWS = ColorsForMap.Yellow;
            Properties.Local.ColorsMap.ColorSectorRI = ColorsForMap.Yellow;
            Properties.Local.ColorsMap.ColorSectorRJ = ColorsForMap.Yellow;
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

        private void Properties_OnLanguageChanged(object sender, Languages e)
        {
            TranslateMapLayout(e);
        }

        private void Properties_OnPathMapChanged(object sender, PathMap e)
        {

        }

        #endregion

        private void RastrMap_OnOnPCPosition(object sender, Location e)
        {
            try
            {
                WGSCoordinate coord = new WGSCoordinate
                {
                    Latitude = Math.Round(e.Latitude, 6),
                    Longitude = Math.Round(e.Longitude, 6)
                };

                ControlPost controlPost = new ControlPost(0);
                controlPost.Coordinate = coord;

                RastrMap.UpdatePC(controlPost);

                DrawSector(new Coord() { Latitude = e.Latitude, Longitude = e.Longitude }, new AntennasMessage(), 1);

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


        #region Draw

        private const string partOfPath = @"\Resources\";
        private MapObjectStyle mapObjectStyleSqare;
        private MapObjectStyle mapObjectStyleTriangle;
        private MapObjectStyle mapObjectStyleStation;
        private double scale = 0.2;


        public void DrawPolygonOfLineOfSight()
        {
            if (polygon.Count > 0)
                RastrMap.mapControl.AddPolygon(polygon, new Color(124, 252, 0, 100));
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


        public void DrawSector(Coord point, AntennasMessage antennasMessage, int id)
        {
            DefinderJammingPoint definderJammingPoint = RastrMap.DefinderJammingPoint.Where(x=> x.ID == id).FirstOrDefault();
            if (definderJammingPoint == null)
                definderJammingPoint = new DefinderJammingPoint(id);

            definderJammingPoint.Coordinate = new WGSCoordinate { Latitude = point.Latitude, Longitude = point.Longitude, Altitude = (float)point.Altitude};
            for (int i = 0; i < 8; i++ )
            {
                var antena = new Antenna();

                try
                {                    
                    antena.Sector = 20;
                    antena.Direction = antennasMessage.Lpa[i];
                    antena.BrushAntenna = Color.Green;
                    antena.PenAntenna = new Pen(Color.Green, 2);
                    antena.Active = true;
                    RastrMap.ListRangesParam.Add( new RangesParam(10, 100000, 0, 255, 0));
                }
                catch(ArgumentOutOfRangeException ex)
                {
                    antena.Sector = 0;
                    antena.Direction = 0;
                    antena.BrushAntenna = Color.Transparent;
                    antena.PenAntenna = new Pen(Color.Transparent, 0);
                    antena.Active = true;
                    RastrMap.ListRangesParam.Add(new RangesParam(10, 100000, 0, 0, 0));
                }

                
                antena.Radius = 10000;

                definderJammingPoint.AntennaDefinder.Add(antena);
                definderJammingPoint.AntennaJamming.Add(antena);
                
            }

            RastrMap.rasterViewModel.DefinderJammingPoint.Clear();
            RastrMap.rasterViewModel.DefinderJammingPoint.Add(definderJammingPoint);
            RastrMap.UpdateDFP(definderJammingPoint);
        }

        private int CheckAngle(int angle)
        {
            if (angle < 0)
                angle = 360 + angle;
            if (angle > 360)
                angle = angle - 360;
            return angle;
        }

        #endregion


        private void ToggleButton_DownPanel_Unchecked(object sender, RoutedEventArgs e)
        {
            OnNeedToRedrawMapJojects(this, e);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}