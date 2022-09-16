using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TransmissionLib.GrpcTransmission;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void GrpcClient_ConnectionStateChanged(object sender, bool e)
        {
            Dispatcher.Invoke(() => {
                mainWindowViewSize.ConnectionStatesGrpcServer = WPFControlConnection.ConnectionStates.Disconnected;
            });
        }

        private void GrpcClient_OnGetTextMessage(object sender, string e)
        {
            DrawMessageToChat(new UserControl_Chat.Message() { MessageFiled = e});
        }

        private void EdServer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Properties.Local.EdServer.Ethernet):
                    InitializeODConnection_Ethernet();
                    break;
                case nameof(Properties.Local.EdServer.Viper1):
                    InitializeODConnection_Viper();
                    break;
                case nameof(Properties.Local.EdServer.Robustel1):
                    InitializeODConnection_3G_4G();
                    break;

            }
        }

    }
}
