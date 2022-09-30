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
            UpdateSelectedStationModel(lASP);

            Task.Factory.StartNew(() =>
            {
                ConnectionStates connectionStates = IsChosenConnectionConnected(SelectedByConnectionTypeClient1);
                Dispatcher.Invoke(() => mainWindowViewSize.ConnectionStatesGrpcServer1 = connectionStates);
            });
        }

        private void GrpcServer2_ButServerClick(object sender, RoutedEventArgs e)
        {
            UpdateSelectedStationModel(lASP);

            Task.Factory.StartNew(() =>
            {
                ConnectionStates connectionStates = IsChosenConnectionConnected(SelectedByConnectionTypeClient2);
                Dispatcher.Invoke(() => mainWindowViewSize.ConnectionStatesGrpcServer2 = connectionStates);      
            });
        }




        private ConnectionStates IsChosenConnectionConnected(SelectedStationModel grpcClientModel)
        {
            if(grpcClientModel.SelectedConnectionObject == null) return ConnectionStates.Disconnected;

            if (grpcClientModel.SelectedConnectionObject.IsConnected)
            {
                grpcClientModel.SelectedConnectionObject.AbortConnection();
                return ConnectionStates.Disconnected;
            }
            else
            {
                grpcClientModel.SelectedConnectionObject.Connect();
                if(grpcClientModel.SelectedConnectionObject.IsConnected)
                    return ConnectionStates.Connected;
                else return ConnectionStates.Disconnected;
            }
        }

    }
}
