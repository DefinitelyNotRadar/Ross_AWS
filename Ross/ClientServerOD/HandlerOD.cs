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
                //var findRec = 
                var rec = this.lASP.Find(t => t.Id == client.ServerAddress)?.Clone();
                if (rec == null) return;
                rec.IsConnect = e ? Led.Green : Led.Empty;

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    ucASP.ChangeASP(rec.Id, rec);
                    UpdateEvaTableConnection(rec);
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


        private async Task Poll_Station(GrpcClient selectedStation)
        {
            //Task task1 = new Task(() =>
            //    {

            //});
            //task1.Start();
            await PollStation(selectedStation).ConfigureAwait(false);
        }


        private async Task PollStation(GrpcClient selectedStation)
        {
            if (selectedStation == null) return;

            await ReadAsp(await selectedStation.GetAsps().ConfigureAwait(false), selectedStation).ConfigureAwait(false);

            await Task.Delay(1000);

            await ReadRecord(await selectedStation.GetFwsElintDistribution().ConfigureAwait(false), NameTable.TableReconFWS);

            await Task.Delay(1000);

            await ReadRecord(await selectedStation.GetFhssElint().ConfigureAwait(false), NameTable.TableReconFHSS);

            await Task.Delay(1000);

            await ReadRecord(await selectedStation.GetFwsJamming().ConfigureAwait(false), NameTable.TempSuppressFWS);

            await Task.Delay(1000);

            await ReadRecord(await selectedStation.GetFhssJamming().ConfigureAwait(false), NameTable.TempSuppressFHSS);

            await Task.Delay(1000);

            //await ReadStationCoord(selectedStation).ConfigureAwait(false);
            //await ReadAntenasDirections(selectedStation).ConfigureAwait(false);
            await SynchronizeTime(selectedStation).ConfigureAwait(false);

            //TODO: try continueWith
        }

        private async Task ReadRecord(object table, NameTable nameTable)
        {
            //TODO: переделать под несколько станций?

            if (nameTable == NameTable.TempSuppressFWS || nameTable == NameTable.TempSuppressFHSS)
            {
                OnClearRecordsByFilter(this, nameTable);
            }

            if (table == null || clientDB == null)
                return;

            var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(nameTable).ListRecords;
            var loadDB = await clientDB.Tables[nameTable].LoadAsync<AbstractCommonTable>().ConfigureAwait(false);
            var fromDB = loadDB.Select(t => t.Id).ToList();
            foreach (var record in recordsToDB)
            {
                if (fromDB != null && fromDB.Contains(record.Id))
                {
                    await clientDB.Tables[nameTable].ChangeAsync(record).ConfigureAwait(false);
                }
                else
                {
                    await clientDB.Tables[nameTable].AddAsync(record).ConfigureAwait(false);
                }
            }
        }

        private async Task ReadAsp(object table, GrpcClient selectedStation)
        {
            //Dispatcher.Invoke(() =>
            //{
                if (table == null || clientDB == null)
                    return;

                var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(NameTable.TableASP).ToList<TableASP>();
                var fromDB = this.lASP;
                var idList = fromDB.Select(t => t.Id).ToList();
                foreach (var record in recordsToDB)
                {
                    if (fromDB != null && idList.Contains(record.Id))
                    {
                        var rec = fromDB.First(t => t.Id == record.Id);
                        ConvertAspToRoss(rec, record, recordsToDB);
                        await clientDB.Tables[NameTable.TableASP].ChangeAsync(rec).ConfigureAwait(false);
                    }
                    else
                    {
                        var newRec = new TableASP();
                        ConvertAspToRoss(newRec, record, recordsToDB);
                        await clientDB.Tables[NameTable.TableASP].AddAsync(newRec).ConfigureAwait(false);
                    }
                }

            //});

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

        private async Task ReadStationCoord(GrpcClient selectedStation)
        {
            var table = await selectedStation.GetCoordinates().ConfigureAwait(false);
            var coord = (table as Any).Unpack<TransmissionPackageGroza934.CoordMessage>();

            if (coord == null) return;


            foreach (var asp in lASP)
            {
                if (asp.Id == selectedStation.ServerAddress)
                {
                    asp.Coordinates = new Coord() { Latitude = coord.Latitude, Longitude = coord.Longitude };
                }

                if (clientDB != null)
                {
                    await clientDB.Tables[NameTable.TableASP].ChangeAsync(asp).ConfigureAwait(false);
                }

            }

            Dispatcher.Invoke(() =>
            {
                mapLayout.DrawStation(new Coord() { Latitude = coord.Latitude, Longitude = coord.Longitude});
            });
        }

        private async Task ReadAntenasDirections(GrpcClient selectedStation)
        {
            var table = await selectedStation.GetAntennasDirection().ConfigureAwait(false);
            var directions = (table as Any).Unpack<TransmissionPackageGroza934.AntennasMessage>();
            //в Lpa 10 элементов = 10 литер. 
            //1=3,2=4, 5-9(есть варианты 5-7,5-10, 10)
            //C
        }

        private async void GrpcClient_OnGetTextMessage(object sender, string e)
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
            Dispatcher.Invoke(() =>
            {
                newWindow.DrawReceivedMessage((sender as GrpcClient).ServerAddress, e);
            });

            if (clientDB == null) return;
            var message = new TableChatMessage() { SenderAddress = (sender as GrpcClient).ServerAddress, ReceiverAddress = clientAddress, Time = DateTime.Now, Status = ChatMessageStatus.Delivered, Text = e };
            await clientDB.Tables[NameTable.TableChat].AddAsync(message).ConfigureAwait(false);
        }


        public async Task Client_ConfirmLastMessage(int station)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    newWindow.ConfirmSentMessage(station);
                });
                

                if (clientDB == null) return;

                await Task.Delay(100); //TODO: придумать что-то поуниверсальней
                var last = lChatMessages.Last(t => t.ReceiverAddress == station);
                last.Status = ChatMessageStatus.Delivered;
                await clientDB.Tables[NameTable.TableChat].ChangeAsync(last).ConfigureAwait(false);
            }
            catch
            { }
        }

        public async Task SynchronizeTime(GrpcClient selectedStation)
        {
             await selectedStation.SendLocalTime(DateTime.Now).ConfigureAwait(false);
        }

        public async Task<ExecutiveDF?> Client_GetExecutiveDF(GrpcClient selectedStation, int stationId, double frequency, float band)
        {
            var result = await selectedStation.GetExecutiveDF(stationId, frequency, band).ConfigureAwait(false);
            return result;
        }

        public async Task<QuasiSimultaneousDF?> Client_GetQuasiSimultaneousDF(GrpcClient selectedStation, int stationId, double frequency, float band)
        {
            var result = await selectedStation.GetQuasiSimultaneousDF(stationId, frequency, band).ConfigureAwait(false);
            return result;
        }
    }
}
