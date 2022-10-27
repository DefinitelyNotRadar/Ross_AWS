using Database934;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Mapsui.Styles;
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
    using DataTransferModels.DB;

    public partial class MainWindow : Window
    {
        public event EventHandler<string> OnSendMessage;

        private void GrpcClient_ConnectionStateChanged1(object sender, bool e)
        {
            if (e)
            {
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


            this.ChangeAspConnectionStatus(sender as GrpcClient, e);
        }

        private void ChangeAspConnectionStatus(GrpcClient client, bool e)
        {
            try
            {
                var rec = this.lASP.Find(t => t.Id == client.ServerAddress).Clone();
                if (rec == null) return;
                rec.IsConnect = e ? Led.Green : Led.Empty;
                //ChangeASP(int id, TableASP replaceASP)
                //this.clientDB?.Tables[NameTable.TableASP].ChangeAsync(rec);
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    ucASP.ChangeASP(rec.Id, rec);
                });
            }
            catch(Exception ex)
            {}
        }

        //private void ChangeAspMode(GrpcClient client, byte mode)
        //{
        //    try
        //    {
        //        var rec = this.lASP.Find(t => t.Id == client.ServerAddress).Clone();
        //        if (rec == null) return;
        //        rec.Mode = mode;
        //        //ChangeASP(int id, TableASP replaceASP)
        //        //this.clientDB?.Tables[NameTable.TableASP].ChangeAsync(rec);
        //        Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
        //        {
        //            ucASP.ChangeASP(rec.Id, rec);
        //        });
        //    }
        //    catch (Exception ex)
        //    { }
        //}

        private void GrpcClient_ConnectionStateChanged2(object sender, bool e)
        {
            if (e)
            {
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
            this.ChangeAspConnectionStatus(sender as GrpcClient, e);
        }


        private void Poll_Station(GrpcClient selectedStation)
        {
            Task task1 = new Task(() =>
            {
                //if (!selectedStation.Ping("ROSS")) return;

                //ReadRecord(selectedStation.GetFwsElint(), NameTable.TempFWS);
                ReadAsp(selectedStation.GetAsps(), selectedStation);

                Task.Delay(1000);
                ReadRecord(selectedStation.GetFwsElintDistribution(), NameTable.TableReconFWS);
                ReadRecord(selectedStation.GetFhssElint(), NameTable.TableReconFHSS);
                ReadRecord(selectedStation.GetFwsJamming(), NameTable.TempSuppressFWS);
                ReadStationCoord(selectedStation);
                ReadAntenasDirections(selectedStation);
                SynchronizeTime(selectedStation);

            });
            task1.Start();
        }

        //private int tempFWSCounter = 0;
        private void ReadRecord(object table, NameTable nameTable)
        {
            //TODO: переделать под несколько станций
            Dispatcher.Invoke(() =>
            { //TODO:удалять по Id станции
                var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(nameTable).ListRecords;
                var fromDB = clientDB?.Tables[nameTable].Load<AbstractCommonTable>().Select(t=>t.Id).ToList();
                foreach(var record in recordsToDB)
                {
                    //foreach(var tableItem in fromDB)
                    //{
                    if(fromDB != null && fromDB.Contains(record.Id))
                    {
                        clientDB?.Tables[nameTable].Change(record);
                    }
                    else
                    {
                        clientDB?.Tables[nameTable].Add(record);
                    }
                    //}
                }
                
            });
        }

        private void ReadAsp(object table, GrpcClient selectedStation)
        {
            Dispatcher.Invoke(() =>
                {
                    var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(NameTable.TableASP).ToList<TableASP>();
                    //var fromDB = clientDB?.Tables[NameTable.TableASP].Load<TableASP>();
                    var fromDB = this.lASP;
                    var idList = fromDB.Select(t => t.Id).ToList();
                    foreach (var record in recordsToDB)
                    {
                        if (fromDB != null && idList.Contains(record.Id))
                        {
                            var rec = fromDB.First(t => t.Id == record.Id);
                            ConvertAspToRoss(rec, record, recordsToDB);
                            clientDB?.Tables[NameTable.TableASP].Change(rec);
                        }
                        else
                        {
                            var newRec = new TableASP();
                            ConvertAspToRoss(newRec, record, recordsToDB);
                            clientDB?.Tables[NameTable.TableASP].Add(newRec);
                        }
                    }

                });

        }

        private void ConvertAspToRoss(TableASP old, TableASP updated, IList<TableASP> updatedAsps)
        {
            old.Id = updated.Id;
            old.IdMission = updated.IdMission;
            old.MatedStationNumber = ChoosePairStation(updated, updatedAsps);
            old.Coordinates = updated.Coordinates;
            old.Mode = updated.Mode;
            old.Letters = updated.Letters;
            old.Role = updated.Role;
            old.LPA10 = updated.LPA10;
            old.LPA13 = updated.LPA13;
            old.LPA24 = updated.LPA24;
            old.LPA510 = updated.LPA510;
            old.LPA57 = updated.LPA57;
            old.LPA59 = updated.LPA59;
            old.BPSS = updated.BPSS;
            old.AntHeightRec = updated.AntHeightRec;
            old.AntHeightSup = updated.AntHeightSup;
            old.RRS1 = updated.RRS1;
            old.RRS2 = updated.RRS2;
            old.Sectors = updated.Sectors;
            old.CallSign = updated.CallSign;
            //return old;
        }

        private int ChoosePairStation(TableASP asp, IList<TableASP> asps)
        {
            TableASP mated;
            switch (asp.Role)
            {
                case RoleStation.Master:
                    mated = asps.FirstOrDefault(t => t.Role == RoleStation.Slave);
                    break;
                case RoleStation.Slave:
                    mated = asps.FirstOrDefault(t => t.Role == RoleStation.Master);
                    break;
                default:
                    mated = default;
                    break;
            }

            return mated.Equals(default(TableASP)) ? 0 : mated.Id;
        }

        private void ReadStationCoord(GrpcClient selectedStation)
        {
            var table = selectedStation.GetCoordinates();
            var coord = (table as Any).Unpack<TransmissionPackageGroza934.CoordMessage>();
            if (coord == null) return;

            Dispatcher.Invoke(() =>
            {
                mapLayout.DrawStation(new Coord() { Latitude = coord.Latitude, Longitude = coord.Longitude});
            });

        }

        private void ReadAntenasDirections(GrpcClient selectedStation)
        {
            var table = selectedStation?.GetAntennasDirection();
            var directions = (table as Any).Unpack<TransmissionPackageGroza934.AntennasMessage>();
            //в Lpa 10 элементов = 10 литер. 
            //1=3,2=4, 5-9(есть варианты 5-7,5-10, 10)
            //C

                foreach (var asp in lASP)
                {
                    if (asp.Id == selectedStation.ServerAddress)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            //mapLayout.DrawSector(asp.Coordinates, directions.Lpa[0], Color.Red);
                            //mapLayout.DrawSector(asp.Coordinates, directions.Lpa[1], Color.Orange);
                            //mapLayout.DrawSector(asp.Coordinates, directions.Lpa[4], Color.Blue);
                            //mapLayout.DrawSector(asp.Coordinates, directions.Lpa[9], Color.White);            
                        });
                    }
                }

        }

        private void GrpcClient_OnGetTextMessage(object sender, string e)
        {
            //List<UserControl_Chat.Message> curMessages = new List<UserControl_Chat.Message>();

            //if (sender is GrpcClient grpcClient)
            //{
            //    curMessages.Add(new UserControl_Chat.Message
            //    {
            //        MessageFiled = e,
            //        Id = grpcClient.ServerAddress,
            //        IsTransmited = true,
            //        IsSendByMe = Roles.Received

            //    });


            //    Dispatcher.Invoke(() =>
            //    {
            //        chatBuble.SetMessage(curMessages[0].MessageFiled);
            //        newWindow.curChat.DrawMessageToChat(curMessages);
            //    });

            //    var message = new TableChatMessage() { SenderAddress = (sender as GrpcClient).ServerAddress, ReceiverAddress = clientAddress, Time = DateTime.Now, Status = ChatMessageStatus.Delivered, Text = e };
            //    clientDB?.Tables[NameTable.TableChat]?.Add(message);
            //}
            newWindow.DrawReceivedMessage((sender as GrpcClient).ServerAddress, e);
            var message = new TableChatMessage() { SenderAddress = (sender as GrpcClient).ServerAddress, ReceiverAddress = clientAddress, Time = DateTime.Now, Status = ChatMessageStatus.Delivered, Text = e };
            clientDB?.Tables[NameTable.TableChat]?.Add(message);
        }




        private void EdServer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case nameof(Properties.Local.EdServer.Viper1):
                    
            //        break;
            //    case nameof(Properties.Local.EdServer.Robustel1):
                    
            //        break;
            //    case nameof(Properties.Local.EdServer.Viper2):
                    
            //        break;
            //    case nameof(Properties.Local.EdServer.Robustel2):
                    
            //        break;

            //}
        }

        public void Client_ConfirmLastMessage(int station)
        {
            try
            {
                newWindow.ConfirmSentMessage(station);

                var last = lChatMessages.Last(t => t.ReceiverAddress == station);
                last.Status = ChatMessageStatus.Delivered;
                clientDB?.Tables[NameTable.TableChat].Change(last);
            }
            catch
            { }
        }

        public void SynchronizeTime(GrpcClient selectedStation)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    selectedStation?.SendLocalTime(DateTime.Now);
                });
        }

        public ExecutiveDF? Client_GetExecutiveDF(GrpcClient selectedStation, int stationId, double frequency, float band)
        {
            return selectedStation?.GetExecutiveDF(stationId, frequency, band);
        }

        public QuasiSimultaneousDF? Client_GetQuasiSimultaneousDF(GrpcClient selectedStation, int stationId, double frequency, float band)
        {
            return selectedStation?.GetQuasiSimultaneousDF(stationId, frequency, band);
        }
    }
}
