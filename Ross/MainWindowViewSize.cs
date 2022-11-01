using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DataTransferModels.DB;
using DLLSettingsControlPointForMap.Model;
using Ross.AddPanel;
using Ross.JSON;
using Ross.Models;
using WPFControlConnection;

namespace Ross
{
    public class MainWindowViewSize : INotifyPropertyChanged
    {
        private MarkSizeWnd markSizeWnd;
        private ConnectionTypeServerOD connectionType1;
        private ConnectionTypeServerOD connectionType2;
        private ConnectionStates connectionStates = ConnectionStates.Disconnected;
        private ConnectionStates connectionStatesGrpcServer1 = ConnectionStates.Disconnected;
        private ConnectionStates connectionStatesGrpcServer2 = ConnectionStates.Disconnected;

        public MainWindowViewSize()
        {
            InitMarkSizeWnd();
        }


        public LocalProperties Local { get; set; }

        public ConnectionStates ConnectionStatesDB
        {
            get => connectionStates;
            set
            {
                if (connectionStates == value) return;
                connectionStates = value;
                OnPropertyChanged();
            }
        }

        public ConnectionTypeServerOD SelectedConnectionType1
        {
            get => connectionType1;
            set
            {
                if (connectionType1 == value) return;
                connectionType1 = value;
                ConnectionStatesGrpcServer1 = ConnectionStates.Disconnected;
                OnPropertyChanged();
            }
        }

        public ConnectionTypeServerOD SelectedConnectionType2
        {
            get => connectionType2;
            set
            {
                if (connectionType2 == value) return;
                connectionType2 = value;
                ConnectionStatesGrpcServer2 = ConnectionStates.Disconnected;
                OnPropertyChanged();
            }
        }

        public ConnectionStates ConnectionStatesGrpcServer1 
        {
            get => connectionStatesGrpcServer1; 
            set
            {
                if (connectionStatesGrpcServer1 == value) return;
                connectionStatesGrpcServer1 = value;
                OnPropertyChanged();
            }
        }

        public ConnectionStates ConnectionStatesGrpcServer2
        {
            get => connectionStatesGrpcServer2;
            set
            {
                if (connectionStatesGrpcServer2 == value) return;
                connectionStatesGrpcServer2 = value;
                OnPropertyChanged();
            }
        }

        public SizeValue sizeChat { get; set; } = new SizeValue() { Current = 0, Visible = false};
        public SizeValue sizeSetting { get; set; } = new SizeValue() { Current = 0, Visible = false};
        public SizeValue sizeTopTable { get; set; }
        public SizeValue sizeDownTable { get; set; }

        public SizeValue sizeLeftDownTable { get; set; }


        private void InitMarkSizeWnd()
        {
            markSizeWnd = SerializerJSON.Deserialize<MarkSizeWnd>(AppDomain.CurrentDomain.BaseDirectory + "SizePanel.json");

            if (markSizeWnd == null)
            {
                markSizeWnd = new MarkSizeWnd();

                sizeChat = new SizeValue(DefaultSize.sizeChat);
                sizeSetting = new SizeValue(DefaultSize.sizeSetting);
                sizeTopTable = new SizeValue(DefaultSize.sizeTopTable);
                sizeDownTable = new SizeValue(DefaultSize.sizeDownTable);
                sizeLeftDownTable = new SizeValue(DefaultSize.sizeLeftDownTable);

                SetDefaultDPanel();
            }

            else
            {
                sizeChat = markSizeWnd.sizeChat;
                sizeSetting = markSizeWnd.sizeSetting;
                sizeTopTable = markSizeWnd.sizeTopTable;
                sizeDownTable = markSizeWnd.sizeDownTable;
                sizeLeftDownTable = markSizeWnd.sizeLeftDownTable;
            }

            sizeChat.PropertyChanged += Size_PropertyChanged;
            sizeSetting.PropertyChanged += Size_PropertyChanged;
            sizeTopTable.PropertyChanged += Size_PropertyChanged;
            sizeDownTable.PropertyChanged += Size_PropertyChanged;
            sizeLeftDownTable.PropertyChanged += Size_PropertyChanged;
        }

        public void SetDefaultDPanel()
        {
            sizeChat.Visible = false;
            sizeSetting.Visible = false;
            sizeTopTable.Visible = true;
            sizeDownTable.Visible = true;
            sizeLeftDownTable.Visible = true;

            sizeChat.Current = DefaultSize.sizeChat;
            sizeSetting.Current = DefaultSize.sizeSetting;
            sizeTopTable.Current = DefaultSize.sizeTopTable;
            sizeDownTable.Current = DefaultSize.sizeDownTable;
            sizeLeftDownTable.Current = DefaultSize.sizeLeftDownTable;

            sizeChat.SetDefault();
            sizeSetting.SetDefault();
            sizeTopTable.SetDefault();
            sizeDownTable.SetDefault();
            sizeLeftDownTable.SetDefault();
        }

        private void Size_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            markSizeWnd.sizeChat = sizeChat;
            markSizeWnd.sizeSetting = sizeSetting;
            markSizeWnd.sizeTopTable = sizeTopTable;
            markSizeWnd.sizeLeftDownTable = sizeLeftDownTable;
            markSizeWnd.sizeDownTable = sizeDownTable;

            markSizeWnd.Serialize(AppDomain.CurrentDomain.BaseDirectory + "SizePanel.json");
        }


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion
    }
}