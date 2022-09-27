using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TransmissionLib.GrpcTransmission;
using WPFControlConnection;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void GrpcServer_ButServerClick(object sender, RoutedEventArgs e)
        {
            if (SelectedByConnectionTypeClient1 == null)
            {
                InitializeODConnection_Viper_1();
                SelectedByConnectionTypeClient1 = grpcClientViper1;
            }
            IsChosenConnectionConnected(SelectedByConnectionTypeClient1, mainWindowViewSize.ConnectionStatesGrpcServer1);
        }

        private void GrpcServer2_ButServerClick(object sender, RoutedEventArgs e)
        {
            if (SelectedByConnectionTypeClient2 == null)
            {
                InitializeODConnection_Viper_2();
                SelectedByConnectionTypeClient1 = grpcClientViper2;
            }
            IsChosenConnectionConnected(SelectedByConnectionTypeClient2, mainWindowViewSize.ConnectionStatesGrpcServer2);
        }

        private void IsChosenConnectionConnected(GrpcClient grpcClient, ConnectionStates connectionStates)
        {
            if(grpcClient == null) return;

            if (grpcClient.IsConnected)
            {
                grpcClient.AbortConnection();
                connectionStates = WPFControlConnection.ConnectionStates.Disconnected;
            }
            else
            {
                grpcClient.Connect(grpcClient.ServerIp, grpcClient.ServerPort);
                connectionStates = WPFControlConnection.ConnectionStates.Connected;
            }
        }

    }
}
