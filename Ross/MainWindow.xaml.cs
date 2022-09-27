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

            SetLocalProperties();
            InitializeMapLayout();

            ChangeLanguage(Properties.Local.Common.Language);
            InitChat();
            InitTables();

            SetLanguageTables(Properties.Local.Common.Language);
            PropViewCoords.ViewCoords = ViewCoordToByte(Properties.Local.CoordinatesProperty.View);

            mainWindowViewSize = new MainWindowViewSize();
            mainWindowViewSize.PropertyChanged += MainWindowViewSize_PropertyChanged;
            DataContext = mainWindowViewSize;
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
            if(e.PropertyName.Equals(nameof(MainWindowViewSize.SelectedConnectionType1)))
            {
                switch (mainWindowViewSize.SelectedConnectionType1)
                {
                    case Models.ConnectionTypeServerOD.Robustel_3G_4G:
                        SelectedByConnectionTypeClient1 = grpcClient_3G_4G1;
                        break;
                    case Models.ConnectionTypeServerOD.Viper_Radio:
                        SelectedByConnectionTypeClient1 = grpcClientViper1;
                        break;
                }
            }
        }

        #region Properties

        private void SetLocalProperties()
        {
            try
            {
                Properties.Local = SerializerJSON.Deserialize<ControlProperties.LocalProperties>("LocalProperties");
                Properties.Local.Common.PropertyChanged += Properties_OnPropertyChanged;
                Properties.Local.Common.IsVisibleAZ = false;
                Properties.Local.EdServer.PropertyChanged += EdServer_PropertyChanged;
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
            grpcClientViper1?.ShutDown();
            grpcClient_3G_4G1?.ShutDown();
            grpcClientViper2?.ShutDown();
            grpcClient_3G_4G2?.ShutDown();

            System.Windows.Threading.Dispatcher.ExitAllFrames();
        }

      
    }
}