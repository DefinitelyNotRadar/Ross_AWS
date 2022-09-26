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
        private GrpcClient SelectedByConnectionTypeClient1;
        private GrpcClient SelectedByConnectionTypeClient2;

        private GrpcClient grpcClientEthernet;
        private GrpcClient grpcClientViper1;
        private GrpcClient grpcClient_3G_4G1;
        private GrpcClient grpcClientViper2;
        private GrpcClient grpcClient_3G_4G2;

        private byte clientAddress = 255;
        private byte serverAddress = 1;
        private int deadlineMs = 10000;

        private void InitializeODConnection_Ethernet()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Ethernet.IpAddress, 30051, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization(grpcClientEthernet);
        }

        private void InitializeODConnection_Viper_1()
        {
            grpcClientViper1 = new GrpcClient(Properties.Local.EdServer.Viper1.IpAddress, Properties.Local.EdServer.Viper1.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization(grpcClientViper1);
        }

        private void InitializeODConnection_3G_4G_1()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Robustel1.IpAddress, Properties.Local.EdServer.Robustel1.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization(grpcClient_3G_4G1);
           
        }

        private void InitializeODConnection_Viper_2()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Viper2.IpAddress, Properties.Local.EdServer.Viper2.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization(grpcClientViper2);
        }

        private void InitializeODConnection_3G_4G_2()
        {
            grpcClientEthernet = new GrpcClient(Properties.Local.EdServer.Robustel2.IpAddress, Properties.Local.EdServer.Robustel2.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization(grpcClient_3G_4G2);
        }


        private void CommonPartInitialization(GrpcClient grpcClient)
        {
            grpcClient.OnTextMessageReceived += GrpcClient_OnGetTextMessage;
            grpcClient.ConnectionStateChanged += GrpcClient_ConnectionStateChanged;          
        }
    }
}
