﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ClientDataBase.Exceptions;
using Database934;
using InheritorsEventArgs;
using ModelsTablesDBLib;
using TableEvents;
using TransmissionLib.GrpcTransmission;

namespace Ross
{
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
                lASP = e.Table;
                ucASP.UpdateASPs(lASP);
                //UpdateSideMenu(lASP);
                //UpdateTableASP4MainPanel(lASP);

                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));
            });
        }

        private void HandlerUpdate_TableSectorRangesRecon(object sender, TableEventArs<TableSectorsRangesRecon> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {

                lSRangeRecon = (from t in e.Table let a = t as TableSectorsRanges select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();

                ucSRangesRecon.UpdateSRanges(lSRangeRecon);
            });
        }

        private void HandlerUpdate_TableSectorRangesSuppr(object sender, TableEventArs<TableSectorsRangesSuppr> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSRangeSuppr = (from t in e.Table let a = t as TableSectorsRanges select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSRangesSuppr.UpdateSRanges(lSRangeSuppr);
            });
        }

        private void HandlerUpdate_TableFreqForbidden(object sender, TableEventArs<TableFreqForbidden> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSpecFreqForbidden = (from t in e.Table let a = t as TableFreqSpec select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSpecFreqForbidden.UpdateSpecFreqs(lSpecFreqForbidden);
            });
        }

        private void HandlerUpdate_TableFreqImportant(object sender, TableEventArs<TableFreqImportant> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSpecFreqImportant = (from t in e.Table let a = t as TableFreqSpec select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSpecFreqImportant.UpdateSpecFreqs(lSpecFreqImportant);
            });
        }

        private void HandlerUpdate_TableFreqKnown(object sender, TableEventArs<TableFreqKnown> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSpecFreqKnown = (from t in e.Table let a = t as TableFreqSpec select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSpecFreqKnown.UpdateSpecFreqs(lSpecFreqKnown);
            });

            //var knownFreqsToServer = ClassDataCommon.ConvertToListAbstractCommonTable(e.Table).ConvertToProto(NameTable.TableFreqKnown);
            //SelectedByConnectionTypeClient.SendTableFreqKnown(knownFreqsToServer); //отправка
        }

        private void HandlerUpdate_TempWFS(object sender, TableEventArs<TempFWS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ucTempFWS.UpdateTempFWS(e.Table,
                    lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
                ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);
            });
        }

        private void HandlerChangeFWS(object sender, TempFWS e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ucTempFWS.ChangeTempFWS(e.Id, e);
                ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);
            });
        }

        private void HandlerAddFWS(object sender, TempFWS e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                ucTempFWS.AddTempFWSs(new List<TempFWS> { e },
                    lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
                ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);
            });
        }

        private void HandlerAddRangeFWS(object sender, TableEventArs<TempFWS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                if (e.Table.Count > 0)
                {
                    ucTempFWS.AddTempFWSs(ExcludeKnownForbiddenFreqs(e.Table),
                        lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());

                    ucTempFWS.ColorFreqImportantForbidden(
                        lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList(),
                        lSpecFreqForbidden.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
                    ucTempFWS.AddToStatusBarCountFI(lSpecFreqImportant
                        .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList().Count);
                }
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
                UpdateSpoofCoord(globalProperties);
            });
        }


        private void UpdateSpoofCoord(GlobalProperties globalProperties)
        {
            if (globalProperties.Latitude > 0 && globalProperties.Longitude > 0)
            {
                //spoof.SpoofControl.SpoofParameters.Latitude = globalProperties.Latitude;
                //spoof.SpoofControl.SpoofParameters.Longitude = globalProperties.Longitude;
            }
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
            });
        }

        private void HandlerUpdate_TempSuppressFWS(object sender, TableEventArs<TempSuppressFWS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                var listTempSuppressFWS = e.Table;
                ucSuppressFWS.UpdateRadioJamState(listTempSuppressFWS);
                //UpdateRadioJamStateStructForBRZ(listTempSuppressFWS);
                var obj = ClassDataCommon.ConvertToListAbstractCommonTable(e.Table).ConvertToProto(NameTable.TableSuppressFWS);
                SelectedByConnectionTypeClient.SendFwsJamming(obj);
            });
        }

        private void HandlerUpdate_TableSuppressFHSS(object sender, TableEventArs<TableSuppressFHSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lSuppressFHSS = (from t in e.Table let a = t select a).ToList()
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSuppressFHSS.UpdateSuppressFHSS(lSuppressFHSS);


                //ListFormer(lSuppressFWS, null);
            });
        }

        private void HandlerUpdate_TableReconFHSS(object sender, TableEventArs<TableReconFHSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lReconFHSS = e.Table;
                ucReconFHSS.UpdateReconFHSS(lReconFHSS);
                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));
            });
        }

        private void HandlerAddRangeReconFHSS(object sender, TableEventArs<TableReconFHSS> e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                lReconFHSS = e.Table;
                ucReconFHSS.AddReconFHSSs(lReconFHSS);
                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));
            });
        }

        private async void LoadTables()
        {
            try
            {
                lASP = await clientDB.Tables[NameTable.TableASP].LoadAsync<TableASP>();
                ucASP.UpdateASPs(lASP);
                //UpdateSideMenu(lASP);
                //UpdateTableASP4MainPanel(lASP);

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


                lReconFHSS = await clientDB.Tables[NameTable.TableReconFHSS].LoadAsync<TableReconFHSS>();
                ucReconFHSS.UpdateReconFHSS(lReconFHSS);
                ucReconFHSS.UpdateASPRP(UpdateASPRPRecon(lASP));
                lSourceFHSS = await clientDB.Tables[NameTable.TableSourceFHSS].LoadAsync<TableSourceFHSS>();
                ucReconFHSS.UpdateSourceFHSS(lSourceFHSS);
                lFHSSReconExcluded = await clientDB.Tables[NameTable.TableFHSSReconExcluded]
                    .LoadAsync<TableFHSSReconExcluded>();
                ucReconFHSS.UpdateTableFHSSReconExcluded(lFHSSReconExcluded);

                ucTempFWS.UpdateTempFWS(await clientDB.Tables[NameTable.TempFWS].LoadAsync<TempFWS>(),
                    lSpecFreqImportant.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());

                var arg = (await clientDB.Tables[NameTable.GlobalProperties].LoadAsync<GlobalProperties>())
                    .FirstOrDefault();
                Properties.Global = arg;


                //UpdateGlobalProperties4MainPanel(arg);
                //UpdateGlobalProperties4LeftRIButtons(arg);
                //UpdateRanges(arg);


                //UpdateSpoofCoord(arg);

                lSuppressFHSS = (await clientDB.Tables[NameTable.TableSuppressFHSS].LoadAsync<TableSuppressFHSS>())
                    .Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
                ucSuppressFHSS.UpdateSuppressFHSS(lSuppressFHSS);
                lFHSSExcludedFreq = await clientDB.Tables[NameTable.TableFHSSExcludedFreq]
                    .LoadAsync<TableFHSSExcludedFreq>();
                ucSuppressFHSS.UpdateFHSSExcludedFreq(lFHSSExcludedFreq);

                try
                {
                    //double anglePA = AnglePA;
                    //double angleRR = AngleRR;


                    var tempGNSS = (await clientDB?.Tables[NameTable.TempGNSS].LoadAsync<TempGNSS>()).FirstOrDefault();

                    //if (anglePA != -1 && angleRR != -1)
                    //{
                    //    Properties.Local.CoordinatesProperty.CoordGPS = tempGNSS.Location;
                    //    Properties.Local.CoordinatesProperty.CompassPA = tempGNSS.CmpPA;
                    //    Properties.Local.CoordinatesProperty.CompassRR = tempGNSS.CmpRR;
                    //    return;
                    //}
                    if (tempGNSS != null)
                    {
                        //tempGNSS.CmpPA = anglePA == -1 ? tempGNSS.CmpPA : anglePA;
                        //tempGNSS.CmpRR = angleRR == -1 ? tempGNSS.CmpRR : angleRR;
                        clientDB.Tables[NameTable.TempGNSS].Change(tempGNSS);
                    }
                    else
                    {
                        var addTempGNSS = new TempGNSS();
                        // addTempGNSS.CmpPA = anglePA == -1 ? addTempGNSS.CmpPA : anglePA;
                        //addTempGNSS.CmpRR = angleRR == -1 ? addTempGNSS.CmpRR : angleRR;

                        clientDB?.Tables[NameTable.TempGNSS].Add(addTempGNSS);
                    }
                }
                catch (Exception)
                {
                }
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
                    Properties.Local.CoordinatesProperty.CompassPA = tempGNSS.CmpPA;

                    if (Properties.Local.CoordinatesProperty.CompassRR != tempGNSS.CmpRR)
                        Properties.Local.CoordinatesProperty.CompassRR = tempGNSS.CmpRR;
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
    }
}