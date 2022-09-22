using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using DataTransferModels.DB;
using DLLSettingsControlPointForMap.Model;
using Ross.AddPanel;
using Ross.JSON;
using Ross.Map;
using Ross.Models;
using WPFControlConnection;

namespace Ross
{
    public class MainWindowViewSize : INotifyPropertyChanged
    {
        private MarkSizeWnd markSizeWnd;
        private ConnectionTypeServerOD connectionType;
        private ConnectionStates connectionStates = ConnectionStates.Disconnected;
        private ConnectionStates connectionStatesGrpcServer = ConnectionStates.Disconnected;

        public MainWindowViewSize()
        {
            InitMarkSizeWnd();
        }


        public LocalProperties Local { get; set; }

        public ConnectionTypeServerOD SelectedConnectionType
        {
            get => connectionType;
            set
            {
                if (connectionType == value) return;
                connectionType = value;
                ConnectionStatesGrpcServer = ConnectionStates.Disconnected;
                OnPropertyChanged();
            }
        }

        public ConnectionStates ConnectionStatesDB 
        {
            get => connectionStates; 
            set
            {
                if(connectionStates == value) return;
                connectionStates = value;
                OnPropertyChanged();
            }
        } 

        public ConnectionStates ConnectionStatesGrpcServer 
        {
            get => connectionStatesGrpcServer; 
            set
            {
                if (connectionStatesGrpcServer == value) return;
                connectionStatesGrpcServer = value;
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
            markSizeWnd =
                SerializerJSON.Deserialize<MarkSizeWnd>(AppDomain.CurrentDomain.BaseDirectory + "SizePanel.json");

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