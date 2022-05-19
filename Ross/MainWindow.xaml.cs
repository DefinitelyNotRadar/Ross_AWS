﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DLLSettingsControlPointForMap.Model;
using Ross.AddPanel;
using Ross.JSON;
using Ross.Map;
using UserControl_Chat;

namespace Ross
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private MapLayout mapLayout;
        MainWindowViewSize MainWindowViewSize;


        public MainWindow()
        {
            InitializeComponent();

            ucSRangesRecon.AddSRange(new ModelsTablesDBLib.TableSectorsRanges());
            ucASP.AddASP(new ModelsTablesDBLib.TableASP());


            SetLocalProperties();

            SetLanguageTables(Properties.Local.Common.Language);
            SetLanguageConnectionPanel(Properties.Local.Common.Language);
            InitializeMapLayout();
            SetLanguageMapLayout(Properties.Local.Common.Language);

            SetChatSettings();

            MainWindowViewSize = new MainWindowViewSize();
            this.DataContext = MainWindowViewSize;
        }

        #region Map window

        private void InitializeMapLayout()
        {
            mapLayout = new MapLayout();
            mapLayout.Closing += MapLayout_Closing;
        }

        private void ToggleButton_Map_Click(object sender, RoutedEventArgs e)
        {

            if (mapLayout.IsVisible)
                mapLayout.Hide();
            else
                mapLayout.Show();
        }

        private void MapLayout_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ToggleButton_Map.IsChecked = false;
        }

        #endregion



        #region Chat

        private void SetChatSettings()
        {
            List<StationClassForChat> testList = new List<StationClassForChat>()
            {
                new StationClassForChat(0,"ПУ", true),
            };
            UserControlChat.InitStations(testList);
            Events.OnSendStationsMessage += ReturnApprovedMessages;
        }

        public void ReturnApprovedMessages(List<Message> stationsMessages)
        {
            foreach (Message curStationsMessage in stationsMessages)
            {
                //curStationsMessage.IsTransmited = true;
                curStationsMessage.SenderName = "Извинитей пожалуйстович";
                curStationsMessage.MessageFontSize = 20;
                curStationsMessage.SenderNameFontSize = 15;
                curStationsMessage.MessageFiled = curStationsMessage.MessageFiled;

                //curStationsMessage.IsTransmited = false;
            }
            UserControlChat.DrawMessageToChat(stationsMessages);
        }

        #endregion



        #region Properties

        private void SetLocalProperties()
        {
            try
            {
                Properties.Local = SerializerJSON.Deserialize<ModelsTablesDBLib.LocalProperties>("LocalProperties");
                Properties.Local.Common.PropertyChanged += Properties_OnPropertyChanged;
                Properties.Local.Common.IsVisibleAZ = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Properties_OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(Properties.Local.Common.AccessARM))
                mapLayout.MapProperties.Local.Common.Access = ((AccessTypes)(byte)Properties.Local.Common.AccessARM);
        }

        private void Properties_OnUpdateLocalProperties(object sender, ModelsTablesDBLib.LocalProperties e)
        {

        }

        private void Properties_OnPasswordChecked(object sender, bool e)
        {
            mapLayout.MapProperties.Local.Common.Access = e ? AccessTypes.Admin : AccessTypes.User;
        }

        private void Properties_OnUpdateLocalProperties_1(object sender, ModelsTablesDBLib.LocalProperties e)
        {
            SerializerJSON.Serialize<ModelsTablesDBLib.LocalProperties>(e, "LocalProperties");
        }

        #endregion


        private void ToggleButton_Setting_UnChecked(object sender, RoutedEventArgs e)
        {
            //columnSettings.Width = GridLength.Auto;
        }

        private void ToggleButton_TS_Unchecked(object sender, RoutedEventArgs e)
        {
            //columnChat.Width = GridLength.Auto;
        }

      
    }
}
