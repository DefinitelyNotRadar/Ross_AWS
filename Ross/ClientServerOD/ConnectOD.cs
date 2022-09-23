using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TransmissionLib.GrpcTransmission;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void GrpcServer_ButServerClick(object sender, RoutedEventArgs e)
        {
            if (SelectedByConnectionTypeClient == null)
            {
                InitializeODConnection_Ethernet();
                SelectedByConnectionTypeClient = grpcClientEthernet;
            }
            IsChosenConnectionConnected(SelectedByConnectionTypeClient);
        }

        private void IsChosenConnectionConnected(GrpcClient grpcClient)
        {
            if(grpcClient == null) return;

            if (grpcClient.IsConnected)
            {
                grpcClient.AbortConnection();
                mainWindowViewSize.ConnectionStatesGrpcServer = WPFControlConnection.ConnectionStates.Disconnected;
            }
            else
            {
                grpcClient.Connect(grpcClient.ServerIp, grpcClient.ServerPort);
                mainWindowViewSize.ConnectionStatesGrpcServer = WPFControlConnection.ConnectionStates.Connected;
            }
        }

    }
}
