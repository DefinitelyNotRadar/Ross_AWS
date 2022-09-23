using Database934;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using TransmissionLib.GrpcTransmission;
using NameTable = ModelsTablesDBLib.NameTable;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void GrpcClient_ConnectionStateChanged(object sender, bool e)
        {
            if (e)
            {
                Poll();
                Dispatcher.Invoke(() =>
                {
                    mainWindowViewSize.ConnectionStatesGrpcServer = WPFControlConnection.ConnectionStates.Connected;
                });
            }
            else
            {
                //SelectedByConnectionTypeClient.AbortConnection();
                Dispatcher.Invoke(() =>
                {
                    mainWindowViewSize.ConnectionStatesGrpcServer = WPFControlConnection.ConnectionStates.Disconnected;
                });
            }
        }

        private void Poll()
        {
            Task task1 = new Task(() =>
            {
                if (!SelectedByConnectionTypeClient.Ping("")) return;

                ReadRecord(SelectedByConnectionTypeClient.GetFwsElint(), NameTable.TableReconFWS);
                ReadRecord(SelectedByConnectionTypeClient.GetFhssElint(), NameTable.TableReconFHSS);
                ReadRecord(SelectedByConnectionTypeClient.GetAsps(), NameTable.TableASP);   
            });
        }

        private void ReadRecord(object table, NameTable nameTable)
        {
            Dispatcher.Invoke(() =>
            {
                var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(nameTable).ListRecords;
                clientDB?.Tables[nameTable].AddRange(recordsToDB);
            });
        }

        private void ReadStationCoord()
        {
            var table = SelectedByConnectionTypeClient.GetCoordinates();
            var coord = (table as Any).Unpack<TransmissionPackageGroza934.CoordMessage>();

            Dispatcher.Invoke(() =>
            {
                mapLayout.DrawStation(new Coord() { Latitude = coord.Latitude, Longitude = coord.Longitude});
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
