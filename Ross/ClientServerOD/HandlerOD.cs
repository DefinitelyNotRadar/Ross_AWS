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
    using Database934;
    using DataTransferModels.DB;
    using Ross.Models;
    using TableEvents;

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


        
           
        

        private async void UpdateRanges(bool isPreConnected)
        {
            if (Properties.Local.Common.IsUpdateTablesOnStationsAfterConnect)
            {
                var lSRangeRecon = await clientDB.Tables[NameTable.TableSectorsRangesRecon].LoadAsync<TableSectorsRanges>();
                var lSRangeSuppr = await clientDB.Tables[NameTable.TableSectorsRangesSuppr].LoadAsync<TableSectorsRanges>();
                var lSpecFreqForbidden = await clientDB.Tables[NameTable.TableFreqForbidden].LoadAsync<TableFreqSpec>();
                var lSpecFreqImportant = await clientDB.Tables[NameTable.TableFreqImportant].LoadAsync<TableFreqSpec>();
                var lSpecFreqKnown = await clientDB.Tables[NameTable.TableFreqKnown].LoadAsync<TableFreqSpec>();
                var lSuppressFWS = await clientDB.Tables[NameTable.TableSuppressFWS].LoadAsync<TableSuppressFWS>();
                var lSuppressFHSS = await clientDB.Tables[NameTable.TableSuppressFHSS].LoadAsync<TableSuppressFHSS>();

                 SendToEachStation(lSRangeRecon, NameTable.TableSectorsRangesRecon, isPreConnected);
                 SendToEachStation(lSRangeSuppr, NameTable.TableSectorsRangesSuppr, isPreConnected);
                 SendToEachStation(lSpecFreqForbidden, NameTable.TableFreqForbidden, isPreConnected);
                 SendToEachStation(lSpecFreqImportant, NameTable.TableFreqImportant, isPreConnected);
                 SendToEachStation(lSpecFreqKnown, NameTable.TableFreqKnown, isPreConnected);

                 SendToEachStation(lSuppressFWS, NameTable.TableSuppressFWS, isPreConnected);
                 SendToEachStation(lSuppressFHSS, NameTable.TableSuppressFHSS, isPreConnected);
            }
        }

        private async void ChangeAspConnectionStatus(GrpcClient client, bool e)
        {
            try
            {
                var rec = this.lASP.Find(t => t.Id == client.ServerAddress)?.Clone();
                if (rec == null) return;

                rec.IsConnect = e ? Led.Green : Led.Empty;

                Dispatcher.Invoke(() => {
                    ucASP.ChangeASP(rec.Id, rec);
                });

                if (e)
                {
                    UpdateRanges(e);
                    await Poll_Station(client);
                }
            }
            catch (Exception ex)
            { }
        }


        private void ChangeAspConnectionStatus(SelectedStationModel client, bool e)
        {
            try
            {
                //var findRec = 
                var rec = this.lASP.Find(t => t.Id == client.SelectedConnectionObject.ServerAddress)?.Clone();
                if (rec == null) return;

                rec.IsConnect = e ? Led.Green : Led.Empty;              

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)async delegate
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


        private async Task Poll_Station(GrpcClient selectedStation)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    LoadingGiff.Visibility = Visibility.Visible;
                });


                await PollStation(selectedStation).ConfigureAwait(false);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    LoadingGiff.Visibility = Visibility.Collapsed;
                });
            }
        }


        private async Task PollStation(GrpcClient selectedStation)
        {

            if (selectedStation != null)
            {

                await ReadAsp(await selectedStation.GetAsps().ConfigureAwait(false), selectedStation).ConfigureAwait(false);

                await Task.Delay(1000);

                //await ReadRecord(await selectedStation.GetFwsElintDistribution().ConfigureAwait(false), NameTable.TableReconFWS);
                await ReadRecordFWS(await selectedStation.GetFwsElintDistribution().ConfigureAwait(false));

                await Task.Delay(1000);

                //await ReadRecord(await selectedStation.GetFhssElint().ConfigureAwait(false), NameTable.TableReconFHSS);
                await ReadRecordFHSS(await selectedStation.GetFhssElint().ConfigureAwait(false));

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
                await Task.Delay(200);
            }
        }

        private object locker = new object();

        private async Task ReadRecordFWS(object table)
        {
            if (table == null || clientDB == null)
                return;

            var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(NameTable.TableReconFWS).ToList<TableReconFWS>();
            var loadDB = await clientDB.Tables[NameTable.TableReconFWS].LoadAsync<TableReconFWS>().ConfigureAwait(false);
            //var fromDB = loadDB.Select(t => t.Id).ToList();
            foreach (var record in recordsToDB)
            {
                record.Id = 0;
                if (record.ListJamDirect.Count != 0)
                {
                    foreach (var jamDirect in record.ListJamDirect)
                    {
                        jamDirect.ID = 0;
                        if (jamDirect.JamDirect.Bearing > 360)
                            jamDirect.JamDirect.Bearing = -1F;
                    }
                }

                

                //foreach (var record in table)
                //{
                if (loadDB.Any(r => record.FreqKHz == r.FreqKHz && record.Time == r.Time))
                    continue;
                loadDB.Add(record);
            }

            await UpdateTable(NameTable.TableReconFWS, loadDB);
        }

        private async Task ReadRecordFHSS(object table)
        {
            if (table == null || clientDB == null)
                return;

            var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(NameTable.TableReconFHSS).ToList<TableReconFHSS>();
            var loadDB = await clientDB.Tables[NameTable.TableReconFHSS].LoadAsync<TableReconFHSS>().ConfigureAwait(false);
            //var fromDB = loadDB.Select(t => t.Id).ToList();
            foreach (var record in recordsToDB)
            {
                record.Id = 0;
                //foreach (var record in table)
                //{
                if (loadDB.Any(r => record.FreqMinKHz == r.FreqMinKHz && record.FreqMaxKHz == r.FreqMaxKHz))
                    continue;
                loadDB.Add(record);

            }

            await UpdateTable(NameTable.TableReconFHSS, loadDB);
        }

        //private async Task ReadRecordFWS2(object table)
        //{
        //    if (table == null || clientDB == null)
        //        return;

        //    var recordsToDB = (table as RepeatedField<Any>).ConvertToDBModel(NameTable.TableReconFWS).ToList<TableReconFWS>();
        //    var loadDB = await clientDB.Tables[NameTable.TableReconFWS].LoadAsync<TableReconFWS>().ConfigureAwait(false);
        //    var loadDBJamDirect = loadDB.Select(t=>t.ListJamDirect.Select(s=>s.ID));
        //    //var fromDB = loadDB.Select(t => t.Id).ToList();
        //    foreach (var record in recordsToDB)
        //    {
        //        record.Id = 0;
        //        if (record.ListJamDirect.Count != 0)
        //        {
        //            foreach (var jamDirect in record.ListJamDirect)
        //            {
        //                jamDirect.ID = 0;
        //            }
        //        }

        //        //foreach (var record in table)
        //        //{
        //        var found = loadDB.FirstOrDefault(r => record.FreqKHz == r.FreqKHz);
        //        if (found != null)
        //        {
        //            if(found.ListJamDirect.)
        //            if(record.ListJamDirect.Count!=0)
        //        }

        //        //if ()
        //        //{
        //        //    continue;
        //        //}
        //        loadDB.Add(record);
        //    }

        //    await UpdateTable(NameTable.TableReconFWS, loadDB);
        //}

        private async Task UpdateTable(NameTable table, object updatedTable)
        {
            if (this.clientDB == null) return;

            lock (locker)
            {
                clientDB.Tables[table].Clear();
                //await Task.Delay(500);
                clientDB.Tables[table].AddRange(updatedTable);
            }
        }

        //public void ActuallyUpdateFhssReconTable(int stationNumber, TableReconFHSS[] table)
        //{
        //    var currentTable = clientDB.Tables[NameTable.TableReconFHSS]
        //        .Load<TableReconFHSS>()
        //        .Where(r => r.Id != stationNumber) //todo : here
        //        .ToList();
        //    currentTable.AddRange(table);

        //    await UpdateTable(NameTable.TableReconFHSS, currentTable);
        //    RewriteTable(N, currentTable, 2, "updating master fhss recon");
        //}

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

            if (mated == null)
                return 0;

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
             await selectedStation.SendLocalTime(DateTime.Now.ToUniversalTime()).ConfigureAwait(false);
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
