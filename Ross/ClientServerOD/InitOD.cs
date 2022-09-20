using Database934;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using ModelsTablesDBLib;
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
        private GrpcClient SelectedByConnectionTypeClient;

        private GrpcClient grpcClientEthernet;
        private GrpcClient grpcClientViper;
        private GrpcClient grpcClient_3G_4G;

        private byte clientAddress = 255;
        private byte serverAddress = 1;
        private int deadline = 10;

        private void InitializeODConnection_Ethernet()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Ethernet.IpAddress, Properties.Local.EdServer.Ethernet.Port, deadline, clientAddress, serverAddress);
            CommonPartInitialization(grpcClientEthernet);
        }

        private void InitializeODConnection_Viper()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Viper1.IpAddress, Properties.Local.EdServer.Viper1.Port, deadline, clientAddress, serverAddress);
            CommonPartInitialization(grpcClientViper);
        }

        private void InitializeODConnection_3G_4G()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Robustel1.IpAddress, Properties.Local.EdServer.Robustel2.Port, deadline, clientAddress, serverAddress);
            CommonPartInitialization(grpcClient_3G_4G);
           
        }


        private void CommonPartInitialization(GrpcClient grpcClient)
        {
            grpcClient.OnGetTextMessage += GrpcClient_OnGetTextMessage;
            grpcClient.ConnectionStateChanged += GrpcClient_ConnectionStateChanged;          
        }
    }
}
