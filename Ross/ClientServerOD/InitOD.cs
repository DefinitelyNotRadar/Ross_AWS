using Database934;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using ModelsTablesDBLib;
using Ross.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
        private int deadlineMs = 10000;


        private void InitializationAllConnections()
        {
            SelectedStationModels = new SelectedStationModel[2] { SelectedByConnectionTypeClient1, SelectedByConnectionTypeClient2 };
        }

        private void InitializeODConnection(SelectedStationModel selectedStationModel, string serverIp, int serverPort, string serverIp3G, int serverPort3G, int serverAddress, int slaveId, Stations stations)
        {
            selectedStationModel.IdMaster = serverAddress;
            selectedStationModel.IdSlave = slaveId;

            if (stations == Stations.StationsPair1 || stations == Stations.SinglStation1)
            {
                switch (mainWindowViewSize.SelectedConnectionType1)
                {
                    case ConnectionTypeServerOD.Robustel_3G_4G:
                        selectedStationModel.SelectedConnectionObject = new GrpcClient(serverIp3G.Replace(",", "."), serverPort3G, deadlineMs, clientAddress, serverAddress);
                        break;
                    default:
                        selectedStationModel.SelectedConnectionObject = new GrpcClient(serverIp.Replace(",", "."), serverPort, deadlineMs, clientAddress, serverAddress);
                        break;
                }
                CommonPartInitialization1(SelectedByConnectionTypeClient1.SelectedConnectionObject);
            }
            else
            {
                switch (mainWindowViewSize.SelectedConnectionType2)
                {
                    case ConnectionTypeServerOD.Robustel_3G_4G:
                        selectedStationModel.SelectedConnectionObject = new GrpcClient(serverIp3G.Replace(",", "."), serverPort3G, deadlineMs, clientAddress, serverAddress);
                        break;
                    default:
                        selectedStationModel.SelectedConnectionObject = new GrpcClient(serverIp.Replace(",", "."), serverPort, deadlineMs, clientAddress, serverAddress);
                        break;
                }
                CommonPartInitialization2(SelectedByConnectionTypeClient2.SelectedConnectionObject);
            }
        }

        //private void InitializeODConnection(SelectedStationModel selectedStationModel, string serverIp, int serverPort, byte serverAddress,int slaveId , Stations stations)
        //{ 
        //    selectedStationModel.SelectedConnectionObject = new GrpcClient(serverIp.Replace(",","."), serverPort, deadlineMs, clientAddress, serverAddress);
        //    selectedStationModel.IdMaster = serverAddress;
        //    selectedStationModel.IdSlave = slaveId;

        //    if (stations == Stations.StationsPair1 || stations == Stations.SinglStation1)
        //    {
        //        switch (mainWindowViewSize.SelectedConnectionType1)
        //        {
        //            case ConnectionTypeServerOD.Robustel_3G_4G:
        //                CreateEndPointObject(selectedStationModel, Properties.Local.EdServer.Robustel1);
        //                break;
        //            case ConnectionTypeServerOD.Viper_Radio:
        //            case ConnectionTypeServerOD.Ethernet:
        //                CreateEndPointObject(selectedStationModel, Properties.Local.EdServer.Viper1);
        //                break;
        //            default:
        //                CreateEndPointObject(selectedStationModel, Properties.Local.EdServer.Viper1);
        //                break;
        //        }
        //        CommonPartInitialization1(SelectedByConnectionTypeClient1.SelectedConnectionObject);
        //    }
        //    else
        //    {
        //        switch (mainWindowViewSize.SelectedConnectionType2)
        //        {
        //            case ConnectionTypeServerOD.Robustel_3G_4G:
        //                CreateEndPointObject(selectedStationModel, Properties.Local.EdServer.Robustel2);
        //                break;
        //            case ConnectionTypeServerOD.Viper_Radio:
        //            case ConnectionTypeServerOD.Ethernet:
        //                CreateEndPointObject(selectedStationModel, Properties.Local.EdServer.Viper2);
        //                break;
        //            default:
        //                CreateEndPointObject(selectedStationModel, Properties.Local.EdServer.Viper2);
        //                break;
        //        }
        //        CommonPartInitialization1(SelectedByConnectionTypeClient2.SelectedConnectionObject);
        //    }
        //}
      
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
