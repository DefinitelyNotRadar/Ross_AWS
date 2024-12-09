using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using DLLSettingsControlPointForMap.Model;
using Mapsui.Geometries;
using ModelsTablesDBLib;
using Ross.JSON;
using Ross.Map;
using TableEvents;
using LocalProperties = ModelsTablesDBLib.LocalProperties;

namespace Ross
{
    using Ross.Models;
    using System.Xml;
    using ValuesCorrectLib;

    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewSize mainWindowViewSize;

        private MapLayout mapLayout;

        public MainWindow()
        {
            InitializeComponent();

            TranslatorTables.LoadDictionary(Properties.Local.Common.Language);
            ucSRangesRecon.AddSRange(new TableSectorsRanges());
                      
            InitializationAllConnections();
            SetLocalProperties();
            InitializeMapLayout();

            ChangeLanguage(Properties.Local.Common.Language);
            
            InitTables();
            UpdateButtonsCRRD();

            SetLanguageTables(Properties.Local.Common.Language);
            PropViewCoords.ViewCoords = ViewCoordToByte(Properties.Local.CoordinatesProperty.View);

            mainWindowViewSize = new MainWindowViewSize();
            mainWindowViewSize.PropertyChanged += MainWindowViewSize_PropertyChanged;
            DataContext = mainWindowViewSize;
            
            InitChat();

            if(Properties.Local.Common.IsAutoPollEnabled)
                CyclePollTimerInitialize();
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

        private void MainWindowViewSize_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("SelectedConnectionType1"))
            {
                UpdateSelectedStationModel(lASP, true);
            }

            if (e.PropertyName.Equals("SelectedConnectionType2"))
            {
                UpdateSelectedStationModel(lASP, true);
            }


            //
        }

        #region Properties

        private void SetLocalProperties()
        {
            try
            {
                var lp = SerializerJSON.Deserialize<ControlProperties.LocalProperties>("LocalProperties");

                Properties.Local = lp ?? new ControlProperties.LocalProperties();
                Properties.Local.Common.PropertyChanged += Properties_OnPropertyChanged;
                Properties.Local.Common.IsVisibleAZ = false;
                endPoint = Properties.Local.DbServer.IpAddress + ":" + Properties.Local.DbServer.Port;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

     

        private void Properties_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Properties.Local.Common.AccessARM))
                mapLayout.MapProperties.Local.Common.Access = (AccessTypes)(byte)Properties.Local.Common.AccessARM;
            if (e.PropertyName == nameof(Properties.Local.Common.View))
                mapLayout.SetCoordinateFormat(Properties.Local.Common.View);
            endPoint = Properties.Local.DbServer.IpAddress + ":" + Properties.Local.DbServer.Port;
            UpdateButtonsCRRD();
        }


        private void Properties_OnPasswordChecked(object sender, bool e)
        {
            mapLayout.MapProperties.Local.Common.Access = e ? AccessTypes.Admin : AccessTypes.User;
        }

        private void Properties_OnUpdateLocalProperties_1(object sender, ControlProperties.LocalProperties e)
        {
            SerializerJSON.Serialize(e, "LocalProperties");
        }

        #endregion

        private void Window_Closed(object sender, EventArgs e)
        {
            clientDB?.Disconnect();
            clientDB = null;
            System.Windows.Threading.Dispatcher.ExitAllFrames();
        }

        private async void ToggleButton_Poll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SelectedStationModels)
            {
                if(item.SelectedConnectionObject != null && item.SelectedConnectionObject.IsConnected)
                    await Poll_Station(item.SelectedConnectionObject);
            }
        }

        private void Properties_DefaultEvent(object sender, ControlProperties.BasicProperties.PropertiesType propertiesType)
        {
            if(propertiesType == ControlProperties.BasicProperties.PropertiesType.Local)
            {
                Properties.Local = SerializerJSON.Deserialize<ControlProperties.LocalProperties>("DefaultSettings");
            }
            else if(propertiesType == ControlProperties.BasicProperties.PropertiesType.Global)
            {
                Properties.Global.NumberIri = 10;
                Properties.Global.GnssInaccuracy = 0;

            }
        }
    }
}