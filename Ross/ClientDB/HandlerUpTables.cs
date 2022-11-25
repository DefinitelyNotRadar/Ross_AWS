using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ClientDataBase.Exceptions;
using Database934;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using InheritorsEventArgs;
using ModelsTablesDBLib;
using Ross.Models;
using TableEvents;
using TransmissionLib.GrpcTransmission;

namespace Ross
{
    using UserControl_Chat;

    using WPFControlConnection;

    public partial class MainWindow : Window
    {

        private void HandlerError_ClientDb(object sender, OperationTableEventArgs e)
        {
            MessageBox.Show(e.GetMessage);
        }

        private void HandlerUpdate_TableASP(object sender, TableEventArs<TableASP> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lASP = new List<TableASP>(e.Table);
                ucASP.UpdateASPs(lASP);
                UpdateSideMenu(lASP);
                DrawAllObjects();
                UpdateSelectedStationModel(lASP, false);
                //UpdateTableASP4MainPanel(lASP);
                //ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));
                //ucReconFWS.UpdateASPRP(lASP, lReconFWS);



            });





        }

        private void HandlerUpdate_TableSectorRangesRecon(object sender, TableEventArs<TableSectorsRangesRecon> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {

                lSRangeRecon = (from t in e.Table let a = t as TableSectorsRanges select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();

                ucSRangesRecon.UpdateSRanges(lSRangeRecon);

                SendToEachStation(e.Table, NameTable.TableSectorsRangesRecon);//отправка
            });
        }

        private void HandlerUpdate_TableSectorRangesSuppr(object sender, TableEventArs<TableSectorsRangesSuppr> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSRangeSuppr = (from t in e.Table let a = t as TableSectorsRanges select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSRangesSuppr.UpdateSRanges(lSRangeSuppr);

                SendToEachStation(e.Table, NameTable.TableSectorsRangesSuppr);
            });
        }

        private void HandlerUpdate_TableFreqForbidden(object sender, TableEventArs<TableFreqForbidden> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSpecFreqForbidden = (from t in e.Table let a = t as TableFreqSpec select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSpecFreqForbidden.UpdateSpecFreqs(lSpecFreqForbidden);

                SendToEachStation(e.Table, NameTable.TableFreqForbidden);
            });
        }

        private void HandlerUpdate_TableFreqImportant(object sender, TableEventArs<TableFreqImportant> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSpecFreqImportant = (from t in e.Table let a = t as TableFreqSpec select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSpecFreqImportant.UpdateSpecFreqs(lSpecFreqImportant);

                SendToEachStation(e.Table, NameTable.TableFreqImportant);
            });
        }

        private void HandlerUpdate_TableFreqKnown(object sender, TableEventArs<TableFreqKnown> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSpecFreqKnown = (from t in e.Table let a = t as TableFreqSpec select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSpecFreqKnown.UpdateSpecFreqs(lSpecFreqKnown);

                SendToEachStation(e.Table, NameTable.TableFreqKnown);
            });


        }

        //private void HandlerUpdate_TempWFS(object sender, TableEventArs<TempFWS> e)
        //{
        //    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
        //    {
        //        ucTempFWS.UpdateTempFWS(e.Table,
        //            lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
        //        ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
        //            .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);
        //    });

        //}

        //private void HandlerChangeFWS(object sender, TempFWS e)
        //{
        //    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
        //    {
        //        ucTempFWS.ChangeTempFWS(e.Id, e);
        //        ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
        //            .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);

        //        DrawAllObjects();
        //    });
        //}

        //private void HandlerAddFWS(object sender, TempFWS e)
        //{
        //    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
        //    {
        //        ucTempFWS.AddTempFWSs(new List<TempFWS> { e },
        //            lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
        //        ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
        //            .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);

        //        DrawAllObjects();
        //    });
        //}

        //private void HandlerAddRangeFWS(object sender, TableEventArs<TempFWS> e)
        //{
        //    Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
        //    {
        //        if (e.Table.Count > 0)
        //        {
        //            ucTempFWS.AddTempFWSs(ExcludeKnownForbiddenFreqs(e.Table),
        //                lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());

        //            ucTempFWS.ColorFreqImportantForbidden(
        //                lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList(),
        //                lSpecFreqForbidden.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
        //            ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
        //                .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);
        //        }
        //    });
        //}


        private void HandlerUpdate_TableReconFWS(object sender, TableEventArs<TableReconFWS> e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                lReconFWS = e.Table;
                ucReconFWS.UpdateReconFWS(lReconFWS);
                ucReconFWS.UpdateASPRP(lASP, lReconFWS);
                DrawAllObjects();
            });
        }

        private void HandlerAddRangeReconFWS(object sender, TableEventArs<TableReconFWS> e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                ///////////////////////////////////////////
                //lReconFWS = e.Table;
                //lReconFWS.AddRange(e.Table);
                //ucReconFWS.AddReconFWSs(lReconFWS);
                ///////////////////////////////////////////
                for (int i = 0; i < e.Table.Count; i++)
                {
                    int ind = lReconFWS.FindIndex(x => x.Id == e.Table[i].Id);
                    if (ind != -1)
                    {
                        lReconFWS[ind] = e.Table[i];
                    }
                    else
                    {
                        lReconFWS.Add(e.Table[i]);
                    }
                }
                //ucReconFWS.AddReconFWSs(e.Table);
                ucReconFWS.UpdateReconFWS(lReconFWS);
                ucReconFWS.UpdateASPRP(lASP, lReconFWS);
                ///////////////////////////////////////////
                //lReconFWS = e.Table;
                //ucReconFWS.UpdateReconFWS(lReconFWS);
                //ucReconFWS.UpdateASPRP(lASP, lReconFWS);
                DrawAllObjects();
            });
        }

        private void HandlerUpdate_GlobalProperties(object sender, TableEventArs<GlobalProperties> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                var globalProperties = e.Table.FirstOrDefault();
                Properties.Global = globalProperties;
                //UpdateGlobalProperties4MainPanel(globalProperties);
                //UpdateGlobalProperties4LeftRIButtons(globalProperties);
            });
        }

        private void HandlerUpdate_TableSuppressFWS(object sender, TableEventArs<TableSuppressFWS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSuppressFWS = e.Table;
                ucSuppressFWS.UpdateSuppressFWS(lSuppressFWS.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP)
                    .ToList());
                //ListFormer(lSuppressFWS, null);

                ucSuppressFWS.UpdateSuppressFWS(lSuppressFWS);


                SendToEachStation<TableSuppressFWS>(e.Table, NameTable.TableSuppressFWS);

                DrawAllObjects();
            });
        }

       




        private void HandlerUpdate_TempSuppressFWS(object sender, TableEventArs<TempSuppressFWS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                var listTempSuppressFWS = e.Table;
                ucSuppressFWS.UpdateRadioJamState(listTempSuppressFWS);
                //UpdateRadioJamStateStructForBRZ(listTempSuppressFWS);

            });
        }

        private void HandlerUpdate_TableSuppressFHSS(object sender, TableEventArs<TableSuppressFHSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSuppressFHSS = (from t in e.Table let a = t select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSuppressFHSS.UpdateSuppressFHSS(lSuppressFHSS);



                SendToEachStation(e.Table, NameTable.TableSuppressFHSS);
                DrawAllObjects();
            });
        }

       

        private void HandlerUpdate_TableFHSSExcludedFreq(object sender, TableEventArs<TableFHSSExcludedFreq> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lFHSSExcludedFreq = e.Table;
                ucSuppressFHSS.UpdateFHSSExcludedFreq(lFHSSExcludedFreq);
                //SendToEachStation(e.Table, NameTable.TableFHSSExcludedFreq);

                //var obj = ClassDataCommon.ConvertToListAbstractCommonTable(e.Table).ConvertToProto(NameTable.TableFHSSExcludedFreq);
                //SelectedByConnectionTypeClient1.SelectedConnectionObject.SendFhssJamming(obj);
                //SelectedByConnectionTypeClient2.SelectedConnectionObject?.SendFhssJamming(obj);
                DrawAllObjects();
            });
        }

        private void HandlerUpdate_TableReconFHSS(object sender, TableEventArs<TableReconFHSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lReconFHSS = e.Table;
                ucReconFHSS.UpdateReconFHSS(lReconFHSS);
                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));

                DrawAllObjects();
            });
        }

        private void HandlerAddRangeReconFHSS(object sender, TableEventArs<TableReconFHSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lReconFHSS = e.Table;
                ucReconFHSS.AddReconFHSSs(lReconFHSS);
                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));

                DrawAllObjects();
            });
        }

        private async Task SendToEachStation<T>(List<T> list, NameTable nameTable) where T : AbstractDependentASP
        {
            try
            {
                for (int i = 0; i < SelectedStationModels.Length; i++)
                {
                    if (SelectedStationModels[i].SelectedConnectionObject == null || SelectedStationModels[i].SelectedConnectionObject.IsConnected == false) continue;

                    List<T> forStation = new List<T>();
                    

                    foreach (var listItem in list)
                    {
                        if (SelectedStationModels[i].IdMaster == listItem.NumberASP || SelectedStationModels[i].IdSlave == listItem.NumberASP)
                        {
                            forStation.Add(listItem);
                        }
                    }

                    var obj = ClassDataCommon.ConvertToListAbstractCommonTable(forStation).ConvertToProto(nameTable);

                    switch (nameTable)
                    {
                        case NameTable.TableSuppressFWS:
                            await SelectedStationModels[i].SelectedConnectionObject.SendFwsJamming(obj).ConfigureAwait(false);
                            break;
                        case NameTable.TableSuppressFHSS:
                            await SelectedStationModels[i].SelectedConnectionObject.SendFhssJamming(obj).ConfigureAwait(false);
                            break;
                        case NameTable.TableFreqKnown:
                            await SelectedStationModels[i].SelectedConnectionObject.SendTableFreqKnown(obj).ConfigureAwait(false);
                            break;
                        case NameTable.TableFreqForbidden:
                            await SelectedStationModels[i].SelectedConnectionObject.SendTableFreqForbidden(obj).ConfigureAwait(false);
                            break;
                        case NameTable.TableFreqImportant:
                            await SelectedStationModels[i].SelectedConnectionObject.SendTableFreqImportant(obj).ConfigureAwait(false);
                            break;
                        case NameTable.TableSectorsRangesRecon:
                            await SelectedStationModels[i].SelectedConnectionObject.SendSectorsRangesElint(obj).ConfigureAwait(false);
                            break;
                        case NameTable.TableSectorsRangesSuppr:
                            await SelectedStationModels[i].SelectedConnectionObject.SendSectorsRangesJamming(obj).ConfigureAwait(false);
                            break;
                    }

                }
            }
            catch { }
        }




        private async void LoadTables()
        {
            try
            {
                lASP = await clientDB.Tables[NameTable.TableASP].LoadAsync<TableASP>();
                ucASP.UpdateASPs(lASP);
                UpdateSideMenu(lASP);
                UpdateSelectedStationModel(lASP, false);

                lSRangeRecon = await clientDB.Tables[NameTable.TableSectorsRangesRecon].LoadAsync<TableSectorsRanges>();
                lSRangeSuppr = await clientDB.Tables[NameTable.TableSectorsRangesSuppr].LoadAsync<TableSectorsRanges>();

                ucSRangesRecon.UpdateSRanges(lSRangeRecon.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP)
                    .ToList());
                ucSRangesSuppr.UpdateSRanges(lSRangeSuppr.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP)
                    .ToList());

                lSpecFreqForbidden = await clientDB.Tables[NameTable.TableFreqForbidden].LoadAsync<TableFreqSpec>();
                ucSpecFreqForbidden.UpdateSpecFreqs(lSpecFreqForbidden
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());

                lSpecFreqImportant = await clientDB.Tables[NameTable.TableFreqImportant].LoadAsync<TableFreqSpec>();
                ucSpecFreqImportant.UpdateSpecFreqs(lSpecFreqImportant
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());

                lSpecFreqKnown = await clientDB.Tables[NameTable.TableFreqKnown].LoadAsync<TableFreqSpec>();
                ucSpecFreqKnown.UpdateSpecFreqs(lSpecFreqKnown.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP)
                    .ToList());

                lSuppressFWS = await clientDB.Tables[NameTable.TableSuppressFWS].LoadAsync<TableSuppressFWS>();
                ucSuppressFWS.UpdateSuppressFWS(lSuppressFWS.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP)
                    .ToList());

                lReconFWS = await clientDB.Tables[NameTable.TableReconFWS].LoadAsync<TableReconFWS>();
                ucReconFWS.UpdateReconFWS(lReconFWS);
                ucReconFWS.UpdateASPRP(lASP, lReconFWS);


                lReconFHSS = await clientDB.Tables[NameTable.TableReconFHSS].LoadAsync<TableReconFHSS>();
                ucReconFHSS.UpdateReconFHSS(lReconFHSS);
                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));
                lSourceFHSS = await clientDB.Tables[NameTable.TableSourceFHSS].LoadAsync<TableSourceFHSS>();
                ucReconFHSS.UpdateSourceFHSS(lSourceFHSS);
                lFHSSReconExcluded = await clientDB.Tables[NameTable.TableFHSSReconExcluded]
                    .LoadAsync<TableFHSSReconExcluded>();
                ucReconFHSS.UpdateTableFHSSReconExcluded(lFHSSReconExcluded);


                var arg = (await clientDB.Tables[NameTable.GlobalProperties].LoadAsync<GlobalProperties>())
                    .FirstOrDefault();
                Properties.Global = arg;


                UpdateRanges(arg);

                lSuppressFHSS = (await clientDB.Tables[NameTable.TableSuppressFHSS].LoadAsync<TableSuppressFHSS>())
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSuppressFHSS.UpdateSuppressFHSS(lSuppressFHSS);
                lFHSSExcludedFreq = await clientDB.Tables[NameTable.TableFHSSExcludedFreq]
                    .LoadAsync<TableFHSSExcludedFreq>();
                ucSuppressFHSS.UpdateFHSSExcludedFreq(lFHSSExcludedFreq);

                try
                {

                    var tempGNSS = (await clientDB?.Tables[NameTable.TempGNSS].LoadAsync<TempGNSS>()).FirstOrDefault();

                    if (tempGNSS != null)
                    {
                        clientDB.Tables[NameTable.TempGNSS].Change(tempGNSS);
                    }
                    else
                    {
                        var addTempGNSS = new TempGNSS();
                        // addTempGNSS.CmpPA = anglePA == -1 ? addTempGNSS.CmpPA : anglePA;
                        //addTempGNSS.CmpRR = angleRR == -1 ? addTempGNSS.CmpRR : angleRR;

                        clientDB?.Tables[NameTable.TempGNSS].Add(addTempGNSS);
                    }

                    DrawAllObjects();
                }
                catch (Exception)
                {
                }

                lChatMessages = await clientDB.Tables[NameTable.TableChat].LoadAsync<TableChatMessage>();
                Load_TableChat(this.lChatMessages);
            }
            catch (ExceptionClient exeptClient)
            {
                MessageBox.Show(exeptClient.Message);
            }
            catch (ExceptionDatabase excpetService)
            {
                MessageBox.Show(excpetService.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateSelectedStationModel(List<TableASP> tableASPs, bool IsHardwareChanged)
        {
            int j = -1;

            foreach (var tableASP in tableASPs)
            {
                if (tableASP.Role == RoleStation.Slave)
                {
                    continue;
                }
                j++;
                var oldStation = SelectedStationModels.FirstOrDefault(t => t.IdMaster == tableASP.Id);
                
                if (IsHardwareChanged || oldStation == null || (oldStation != null && oldStation.SelectedConnectionObject != null &&
                    (oldStation.ConnectionTypeServerOD != ConnectionTypeServerOD.Robustel_3G_4G && 
                    (oldStation.IpAddress_interior.Replace(',', '.') != tableASP.AddressIP.Replace(',','.') || oldStation.Port_interior != tableASP.AddressPort)
                    || (oldStation.ConnectionTypeServerOD == ConnectionTypeServerOD.Robustel_3G_4G && 
                    (oldStation.IpAddress_interior_3G4G.Replace(',', '.') != tableASP.AddressIp3G4G.Replace(',', '.') || oldStation.Port_interior_3G4G != tableASP.AddressPort3G4G)))))
                {
                    SelectedStationModels[j].SelectedConnectionObject?.AbortConnection();
                    InitializeODConnection(SelectedStationModels[j], tableASP.AddressIP, tableASP.AddressPort, tableASP.AddressIp3G4G, tableASP.AddressPort3G4G, (byte)tableASP.Id, tableASP.MatedStationNumber, (Stations)j); 
                    Task.Factory.StartNew(() =>
                    {
                        ConnectionStates connectionStates = IsChosenConnectionConnected(SelectedStationModels[j]);
                        if(j==0) Dispatcher.Invoke(() => mainWindowViewSize.ConnectionStatesGrpcServer1 = connectionStates); 
                        else Dispatcher.Invoke(() => mainWindowViewSize.ConnectionStatesGrpcServer2 = connectionStates);
                    });
                    
                }
                else if(oldStation.IdSlave != tableASP.MatedStationNumber)
                    SelectedStationModels[j].IdSlave = tableASP.MatedStationNumber;

                ChangeAspConnectionStatus(SelectedStationModels[j].SelectedConnectionObject, SelectedStationModels[j].SelectedConnectionObject.IsConnected);

                if (j == SelectedStationModels.Length-1) return;
               //TODO: if list. count == 0
            }
        }


        private async void Properties_OnUpdateGlobalProperties(object sender, GlobalProperties e)
        {
            await UpdateCmpRR(e);
            UpdateRanges(e);
        }

        private void HandlerUpdate_TempGNSS(object sender, TableEventArs<TempGNSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                var tempGNSS = e.Table.FirstOrDefault();

                if (tempGNSS != null)
                {
                    tempGNSS.Location.Latitude = Math.Abs(Math.Round(tempGNSS.Location.Latitude, 6));
                    tempGNSS.Location.Longitude = Math.Abs(Math.Round(tempGNSS.Location.Longitude, 6));
                    tempGNSS.Location.Altitude = Math.Round(tempGNSS.Location.Altitude, 0);

                    Properties.Local.CoordinatesProperty.CoordGPS = tempGNSS.Location;
                }
            });
        }


        private async Task UpdateCmpRR(GlobalProperties arg)
        {
            if (arg.SignHeadingAngle)
                if (clientDB != null && clientDB.Tables != null)
                {
                    var tempGNSS = (await clientDB?.Tables[NameTable.TempGNSS].LoadAsync<TempGNSS>()).FirstOrDefault();

                    if (tempGNSS != null)
                    {
                        var headingAngle = (int)Math.Round(tempGNSS.CmpRR) + Properties.Local.CmpRR.CompassСorrection;

                        if (headingAngle >= 360) headingAngle -= 360;

                        arg.HeadingAngle = headingAngle;
                    }
                }

            if (clientDB != null && clientDB.Tables != null)
                await clientDB?.Tables[NameTable.GlobalProperties].AddAsync(arg);
        }

        private void HandlerUpdate_TableChat(object sender, TableEventArs<TableChatMessage> e)
        {
            try
            {
                lChatMessages = new List<TableChatMessage>(e.Table);
                //var messages = new List<UserControl_Chat.Message>();

                //messages = e.Table.OrderBy(t => t.Time).Select(s => new UserControl_Chat.Message()
                //{
                //    Id = s.ReceiverAddress == this.clientAddress ? s.SenderAddress : s.ReceiverAddress,
                //    MessageFiled = s.Text,
                //    IsSendByMe = s.ReceiverAddress == this.clientAddress ? UserControl_Chat.Roles.Received : Roles.SentByMe,
                //    IsTransmited = s.Status == ChatMessageStatus.Delivered,
                //}).ToList();

                //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                //{
                //    newWindow.curChat.DrawMessageToChat(messages);
                //});
            }
            catch { }
        }

        private void Load_TableChat(List<TableChatMessage> e)
        {
            try
            {
                //lChatMessages = new List<TableChatMessage>(e.Table);
                var messages = new List<UserControl_Chat.Message>();

                var chatIds = GetSideMenu();
                messages = e.OrderBy(t => t.Time).Where(t=> chatIds.Contains(t.ReceiverAddress) || chatIds.Contains(t.SenderAddress))
                    .Select(s => new UserControl_Chat.Message()
                                                                        {
                                                                            Id = s.ReceiverAddress == this.clientAddress ? s.SenderAddress : s.ReceiverAddress,
                                                                            MessageFiled = s.Text,
                                                                            IsSendByMe = s.ReceiverAddress == this.clientAddress ? UserControl_Chat.Roles.Received : Roles.SentByMe,
                                                                            IsTransmited = s.Status == ChatMessageStatus.Delivered,
                                                                        }).ToList();

                
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    newWindow.curChat.ClearChatHistory(chatIds);
                    newWindow.curChat.DrawMessageToChat(messages);
                });
            }
            catch { }
        }
    }
}