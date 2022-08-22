using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Instrumentation;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DLLSettingsControlPointForMap;
using DLLSettingsControlPointForMap.Model;
using Mapsui.Styles;
using ModelsTablesDBLib;
using Ross.JSON;
using UIMapRast.Models;
using WpfControlLibrary1;
using WpfMapControl;
using Color = Mapsui.Styles.Color;
using Languages = ModelsTablesDBLib.Languages;
using LocalProperties = DLLSettingsControlPointForMap.Model.LocalProperties;
using Pen = Mapsui.Styles.Pen;

namespace Ross.Map
{
    /// <summary>
    ///     Логика взаимодействия для MapLayout.xaml
    /// </summary>
    public partial class MapLayout : Window
    {
        public MapLayout()
        {
            InitializeComponent();
            Properties.OnDefaultButtonClick += Properties_OnDefaultButtonClick;
            Properties.OnApplyButtonClick += Properties_OnApplyButtonClick;
            LoadSettings();
            InitHotKeys();
            InitEvaTables();
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


        private void ToggleButton_Setting_Unchecked(object sender, RoutedEventArgs e)
        {
            ColumnSettings.Width = new GridLength(0, GridUnitType.Pixel);
        }

        private void ToggleButton_Setting_Checked(object sender, RoutedEventArgs e)
        {
            ColumnSettings.Width = new GridLength(0, GridUnitType.Auto);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }


        

        public void InitEvaTables()
        { 
            evaTable.AddNewItem(new Tabl() {name = "Eva", id = 1});
        }

        #region Hot Keys

        private void InitHotKeys()
        {
            var newCmd = new RoutedCommand();
            newCmd.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(newCmd, OpenEvaTable));
        }

        private void OpenEvaTable(object sender, ExecutedRoutedEventArgs e)
        {
            if (evaTable.Visibility == Visibility.Visible)
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

                DefinderJammingPoint definderJammingPoint = new DefinderJammingPoint(0);
                definderJammingPoint.Coordinate = coord;
                definderJammingPoint.Regime = ERegime.Jamming;


                Pen pen = new Pen();
                pen.Color = Color.FromArgb(50, 0, 255, 0);
                //pen.PenStrokeCap = PenStrokeCap.Square;
                pen.PenStyle = PenStyle.Dash;


                Zone zone = new Zone();
                zone.PenZone = pen;
                zone.Radius = 20000;
                definderJammingPoint.ZoneDefinder = zone;
                

                var collection = new ObservableCollection<DefinderJammingPoint>();
                collection.Add(definderJammingPoint);


                RastrMap.UpdateDFP(collection);
            }
            catch { }
        }

        
    }
}