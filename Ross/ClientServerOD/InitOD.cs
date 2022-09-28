using Database934;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using ModelsTablesDBLib;
using Ross.Models;
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
        SelectedStationModel[] SelectedStationModels;

        private SelectedStationModel SelectedByConnectionTypeClient1 = new SelectedStationModel();
        private SelectedStationModel SelectedByConnectionTypeClient2 = new SelectedStationModel();

        private GrpcClient grpcClientViper1;
        private GrpcClient grpcClient_3G_4G1;
        private GrpcClient grpcClientViper2;
        private GrpcClient grpcClient_3G_4G2;

        private byte clientAddress = 255;
        private byte serverAddress = 1;
        private int deadlineMs = 10000;

     
        private void InitializationAllConnections()
        {
            SelectedStationModels = new SelectedStationModel[2] { SelectedByConnectionTypeClient1, SelectedByConnectionTypeClient2 };
            InitializeODConnection_3G_4G_1();
            InitializeODConnection_3G_4G_2();
            InitializeODConnection_Viper_1();
            InitializeODConnection_Viper_2();
        }

        private void InitializeODConnection_Viper_1()
        {
            grpcClientViper1 = new GrpcClient(Properties.Local.EdServer.Viper1.IpAddress, Properties.Local.EdServer.Viper1.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization1(grpcClientViper1);
        }

        private void InitializeODConnection_3G_4G_1()
        {
            grpcClient_3G_4G1 = new GrpcClient(Properties.Local.EdServer.Robustel1.IpAddress, Properties.Local.EdServer.Robustel1.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization1(grpcClient_3G_4G1);           
        }

        private void InitializeODConnection_Viper_2()
        {
            grpcClientViper2 = new GrpcClient(Properties.Local.EdServer.Viper2.IpAddress, Properties.Local.EdServer.Viper2.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization2(grpcClientViper2);
        }

        private void InitializeODConnection_3G_4G_2()
        {
            grpcClient_3G_4G2 = new GrpcClient(Properties.Local.EdServer.Robustel2.IpAddress, Properties.Local.EdServer.Robustel2.Port, deadlineMs, clientAddress, serverAddress);
            CommonPartInitialization2(grpcClient_3G_4G2);
        }


        private void CommonPartInitialization1(GrpcClient grpcClient)
        {
            grpcClient.OnTextMessageReceived += GrpcClient_OnGetTextMessage;
            grpcClient.ConnectionStateChanged += GrpcClient_ConnectionStateChanged1;          
        }

        private void CommonPartInitialization2(GrpcClient grpcClient)
        {
            grpcClient.OnTextMessageReceived += GrpcClient_OnGetTextMessage;
            grpcClient.ConnectionStateChanged += GrpcClient_ConnectionStateChanged2;
        }

      
    }
}
