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
using UserControl_Chat;
using NameTable = ModelsTablesDBLib.NameTable;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void GrpcClient_ConnectionStateChanged1(object sender, bool e)
        {
            if (e)
            {
                Poll_Station_1();
                Dispatcher.Invoke(() =>
                {
                    mainWindowViewSize.ConnectionStatesGrpcServer1 = WPFControlConnection.ConnectionStates.Connected;
                });
            }
            else
            {
                //SelectedByConnectionTypeClient.AbortConnection();
                Dispatcher.Invoke(() =>
                {
                    mainWindowViewSize.ConnectionStatesGrpcServer1 = WPFControlConnection.ConnectionStates.Disconnected;
                });
            }
        }

        private void GrpcClient_ConnectionStateChanged2(object sender, bool e)
        {
            if (e)
            {
                Poll_Station_2();
                Dispatcher.Invoke(() =>
                {
                    mainWindowViewSize.ConnectionStatesGrpcServer2 = WPFControlConnection.ConnectionStates.Connected;
                });
            }
            else
            {
                //SelectedByConnectionTypeClient.AbortConnection();
                Dispatcher.Invoke(() =>
                {
                    mainWindowViewSize.ConnectionStatesGrpcServer2 = WPFControlConnection.ConnectionStates.Disconnected;
                });
            }
        }


        private void Poll_Station_1()
        {
            Task task1 = new Task(() =>
            {
                if (!SelectedByConnectionTypeClient1.SelectedConnectionObject.Ping("")) return;

                ReadRecord(SelectedByConnectionTypeClient1.SelectedConnectionObject.GetFwsElint(), NameTable.TableReconFWS);
                ReadRecord(SelectedByConnectionTypeClient1.SelectedConnectionObject.GetFhssElint(), NameTable.TableReconFHSS);
                ReadRecord(SelectedByConnectionTypeClient1.SelectedConnectionObject.GetAsps(), NameTable.TableASP);   
            });
        }

        private void Poll_Station_2()
        {
            Task task1 = new Task(() =>
            {
                if (!SelectedByConnectionTypeClient2.SelectedConnectionObject.Ping("")) return;

                ReadRecord(SelectedByConnectionTypeClient2.SelectedConnectionObject.GetFwsElint(), NameTable.TableReconFWS);
                ReadRecord(SelectedByConnectionTypeClient2.SelectedConnectionObject.GetFhssElint(), NameTable.TableReconFHSS);
                ReadRecord(SelectedByConnectionTypeClient2.SelectedConnectionObject.GetAsps(), NameTable.TableASP);
            });
        }

        private void ReadRecord(object table, NameTable nameTable)
        {
            Dispatcher.Invoke(() =>
            {
                var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(nameTable).ListRecords;
                foreach(var record in recordsToDB)
                {
                    foreach(var tableItem in clientDB?.Tables[nameTable].Load<AbstractCommonTable>())
                    {
                        if(tableItem.Id != record.Id)
                        {
                            clientDB?.Tables[nameTable].Add(record);
                            break;
                        }
                    }
                }
                
            });
        }

        private void ReadStationCoord1()
        {
            var table = SelectedByConnectionTypeClient1.SelectedConnectionObject.GetCoordinates();
            var coord = (table as Any).Unpack<TransmissionPackageGroza934.CoordMessage>();

            Dispatcher.Invoke(() =>
            {
                mapLayout.DrawStation(new Coord() { Latitude = coord.Latitude, Longitude = coord.Longitude});
            });

        }

        private void ReadStationCoord2()
        {
            var table = SelectedByConnectionTypeClient2.SelectedConnectionObject.GetCoordinates();
            var coord = (table as Any).Unpack<TransmissionPackageGroza934.CoordMessage>();

            Dispatcher.Invoke(() =>
            {
                mapLayout.DrawStation(new Coord() { Latitude = coord.Latitude, Longitude = coord.Longitude });
            });

        }

        private void GrpcClient_OnGetTextMessage(object sender, string e)
        {
            List<UserControl_Chat.Message> curMessages = new List<UserControl_Chat.Message>();
            curMessages.Add(new UserControl_Chat.Message
            {
                MessageFiled = e,
               // Id = curMessage.SenderId,
                IsTransmited = true,
                IsSendByMe = Roles.Received

            });

           
            Dispatcher.Invoke(() =>
            {
                chatBuble.SetMessage(curMessages[0].MessageFiled);
                newWindow.curChat.DrawMessageToChat(curMessages);
            });
        }

     
       

        private void EdServer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {              
                case nameof(Properties.Local.EdServer.Viper1):
                    InitializeODConnection_Viper_1();
                    break;
                case nameof(Properties.Local.EdServer.Robustel1):
                    InitializeODConnection_3G_4G_1();
                    break;
                case nameof(Properties.Local.EdServer.Viper2):
                    InitializeODConnection_Viper_2();
                    break;
                case nameof(Properties.Local.EdServer.Robustel2):
                    InitializeODConnection_3G_4G_2();
                    break;

            }
        }


    }
}
