using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Management.Instrumentation;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DLLSettingsControlPointForMap;
using DLLSettingsControlPointForMap.Model;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Styles;
using ModelsTablesDBLib;
using Ross.JSON;
using UIMapRast.Models;
using WpfControlLibrary1;
using WpfMapControl;
using Brush = Mapsui.Styles.Brush;
using Color = System.Windows.Media.Color;
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


        public MapLayout()
        {
            InitializeComponent();
            Properties.OnDefaultButtonClick += Properties_OnDefaultButtonClick;
            Properties.OnApplyButtonClick += Properties_OnApplyButtonClick;
            LoadSettings();
            InitHotKeys();

            DataContext = new MapViewModel();

            //mapObjectStyleStation = RastrMap.mapControl.LoadObjectStyle(Environment.CurrentDirectory + partOfPath + "station.png", scale);
        }


        public void SetStationInEvaTable(Tabl newtabl, Tabl oldTable)
        {
            evaTable.DeleteItemModel(oldTable);
            evaTable.AddNewItem(newtabl);
        }

    
        public SettingsControlForMap MapProperties
        {
            get => Properties;
            set => Properties = value;
        }

        #region Map

        public void TranslateMapLayout(Languages languages)
        {
            
        }

        #endregion 

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }


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
            }
            catch { }
        }

        private void RastrMap_OnOnPointPosition(object sender, Location e)
        {
            try
            {
                WGSCoordinate coord = new WGSCoordinate
                {
                    Latitude = Math.Round(e.Latitude, 6),
                    Longitude = Math.Round(e.Longitude, 6)
                };

                DefinderJammingPoint definderJammingPoint = new DefinderJammingPoint(1);
                definderJammingPoint.Coordinate = coord;
                definderJammingPoint.Regime = ERegime.Jamming;


                Pen pen = new Pen();
                pen.Color = Mapsui.Styles.Color.FromArgb(50, 0, 255, 0);
                //pen.PenStrokeCap = PenStrokeCap.Square;
                pen.PenStyle = PenStyle.Dash;


                Zone zone = new Zone();
                zone.PenZone = pen;
                zone.Radius = 2000;
                definderJammingPoint.ZoneDefinder = zone;
                

                var collection = new ObservableCollection<DefinderJammingPoint>();
                collection.Add(definderJammingPoint);


                RastrMap.UpdateDFP(collection);
            }
            catch { }
        }


        #region Draw

        private const string partOfPath = @"\Resources\";
        private MapObjectStyle mapObjectStyleSqare;
        private MapObjectStyle mapObjectStyleTriangle;
        private MapObjectStyle mapObjectStyleStation;
        private double scale = 0.2;

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

        public void DrawSourceFUSS(Coord point, ColorsForMap color)
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

        public void DrawStation(Coord point)
        {
            var p = Mercator.FromLonLat(point.Longitude, point.Latitude);
            RastrMap.mapControl.AddMapObject(mapObjectStyleStation, "", p);
        }

        #endregion

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
    }
}