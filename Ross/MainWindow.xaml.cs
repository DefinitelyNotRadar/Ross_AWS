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

            SetLanguageTables(Properties.Local.Common.Language);
            PropViewCoords.ViewCoords = ViewCoordToByte(Properties.Local.CoordinatesProperty.View);

            mainWindowViewSize = new MainWindowViewSize();
            mainWindowViewSize.PropertyChanged += MainWindowViewSize_PropertyChanged;
            DataContext = mainWindowViewSize;
            
            InitChat();
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
            //UpdateSelectedStationModel(lASP);
        }

        #region Properties

        private void SetLocalProperties()
        {
            try
            {
                Properties.Local = SerializerJSON.Deserialize<ControlProperties.LocalProperties>("LocalProperties");
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
            endPoint = Properties.Local.DbServer.IpAddress + ":" + Properties.Local.DbServer.Port;
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
            grpcClientViper1?.AbortConnection();
            grpcClient_3G_4G1?.AbortConnection();
            grpcClientViper2?.AbortConnection();
            grpcClient_3G_4G2?.AbortConnection();

            System.Windows.Threading.Dispatcher.ExitAllFrames();
        }

        private void ToggleButton_Poll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SelectedStationModels)
            {
                if(item.SelectedConnectionObject != null && item.SelectedConnectionObject.IsConnected)
                    Poll_Station(item.SelectedConnectionObject);

            }
        }
    }
}