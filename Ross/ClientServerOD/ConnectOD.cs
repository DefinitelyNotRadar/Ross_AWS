using Ross.Models;
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
            switch(GrpcConnectionType1.SelectedValue)
            {
                case ConnectionTypeServerOD.Robustel_3G_4G:
                    SelectedByConnectionTypeClient1.SelectedConnectionObject = grpcClient_3G_4G1;
                    break;
                case ConnectionTypeServerOD.Viper_Radio:
                case ConnectionTypeServerOD.Ethernet:
                    SelectedByConnectionTypeClient1.SelectedConnectionObject = grpcClientViper1;
                    break;
            }
            IsChosenConnectionConnected(SelectedByConnectionTypeClient1, mainWindowViewSize.ConnectionStatesGrpcServer1);
        }

        private void GrpcServer2_ButServerClick(object sender, RoutedEventArgs e)
        {

            switch (GrpcConnectionType2.SelectedValue)
            {
                case ConnectionTypeServerOD.Robustel_3G_4G:
                    SelectedByConnectionTypeClient2.SelectedConnectionObject = grpcClient_3G_4G2;
                    break;
                case ConnectionTypeServerOD.Viper_Radio:
                case ConnectionTypeServerOD.Ethernet:
                    SelectedByConnectionTypeClient2.SelectedConnectionObject = grpcClientViper2;
                    break;
            }
            IsChosenConnectionConnected(SelectedByConnectionTypeClient2, mainWindowViewSize.ConnectionStatesGrpcServer2);
        }

        private void IsChosenConnectionConnected(SelectedStationModel grpcClientModel, ConnectionStates connectionStates)
        {
            if(grpcClientModel.SelectedConnectionObject == null) return;

            if (grpcClientModel.SelectedConnectionObject.IsConnected)
            {
                grpcClientModel.SelectedConnectionObject.AbortConnection();
                connectionStates = ConnectionStates.Disconnected;
            }
            else
            {
                grpcClientModel.SelectedConnectionObject.Connect(grpcClientModel.IpAddressMaster, grpcClientModel.PortMaster);
                connectionStates = ConnectionStates.Connected;
            }
        }

    }
}
