﻿using System;
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
using TransmissionLib.GrpcTransmission;
using UserControl_Chat;
using WpfControlLibrary1;
using LocalProperties = ModelsTablesDBLib.LocalProperties;

namespace Ross
{
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

            ucSRangesRecon.AddSRange(new TableSectorsRanges());

            SetLocalProperties();
            InitializeMapLayout();

            ChangeLanguage(Properties.Local.Common.Language);
            InitChat();
            InitTables();


            mainWindowViewSize = new MainWindowViewSize();
            mainWindowViewSize.PropertyChanged += MainWindowViewSize_PropertyChanged;
            DataContext = mainWindowViewSize;
        }

        private void MainWindowViewSize_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals(nameof(MainWindowViewSize.SelectedConnectionType)))
            {
                switch (mainWindowViewSize.SelectedConnectionType)
                {
                    case Models.ConnectionTypeServerOD.Ethernet:
                        SelectedByConnectionTypeClient = grpcClientEthernet;
                        break;
                    case Models.ConnectionTypeServerOD.Robustel_3G_4G:
                        SelectedByConnectionTypeClient = grpcClient_3G_4G;
                        break;
                    case Models.ConnectionTypeServerOD.Viper_Radio:
                        SelectedByConnectionTypeClient = grpcClientViper;
                        break;
                }
            }
        }

        #region Properties

        private void SetLocalProperties()
        {
            try
            {
                Properties.Local = SerializerJSON.Deserialize<LocalProperties>("LocalProperties");
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

        private void Properties_OnUpdateLocalProperties(object sender, LocalProperties e)
        {

        }

        private void Properties_OnPasswordChecked(object sender, bool e)
        {
            mapLayout.MapProperties.Local.Common.Access = e ? AccessTypes.Admin : AccessTypes.User;
        }

        private void Properties_OnUpdateLocalProperties_1(object sender, LocalProperties e)
        {
            e.Serialize("LocalProperties");
        }

        #endregion   
    }
}