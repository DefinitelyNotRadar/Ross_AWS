using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ClientDataBase;
using ClientDataBase.Exceptions;
using ModelsTablesDBLib;
using TableEvents;
using TableOperations;
using ValuesCorrectLib;

namespace Ross
{
    public partial class MainWindow
    {
        // АСП
        public List<TableASP> lASP = new List<TableASP>();
        public List<TableFHSSExcludedFreq> lFHSSExcludedFreq = new List<TableFHSSExcludedFreq>();

        public List<TableFHSSReconExcluded> lFHSSReconExcluded = new List<TableFHSSReconExcluded>();

        // ИРИ ППРЧ
        public List<TableReconFHSS> lReconFHSS = new List<TableReconFHSS>();

        // ИРИ ФРЧ ЦР
        public List<TableReconFWS> lReconFWS = new List<TableReconFWS>();

        public List<TableSourceFHSS> lSourceFHSS = new List<TableSourceFHSS>();

        //public List<ButtonsNAV> lButtonsNAV = new List<ButtonsNAV>();
        // Специальные частоты
        public List<TableFreqSpec> lSpecFreqForbidden = new List<TableFreqSpec>();
        public List<TableFreqSpec> lSpecFreqImportant = new List<TableFreqSpec>();
        public List<TableFreqSpec> lSpecFreqKnown = new List<TableFreqSpec>();

        public List<TableSectorsRanges> lSRangeRecon = new List<TableSectorsRanges>();

        // Сектора и диапазоны
        public List<TableSectorsRanges> lSRangeSuppr = new List<TableSectorsRanges>();

        // ИРИ ППРЧ РП
        public List<TableSuppressFHSS> lSuppressFHSS = new List<TableSuppressFHSS>();

        // ИРИ ФРЧ РП
        public List<TableSuppressFWS> lSuppressFWS = new List<TableSuppressFWS>();
        public List<TableChatMessage> lChatMessages = new List<TableChatMessage>();

        private int selectedASP = -2;

        /// <summary>
        ///     Update tables SectorsRanges, SpecFreqs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UcASP_OnSelectedRow(object sender, ASPEvents e)
        {
            if (e.Id > 0)
            {
                selectedASP = e.Id;
                LoadTablesByFilter(e.Id);
            }
        }

        private async void LoadTablesByFilter(int id)
        {
            try
            {
                lSRangeRecon = await (clientDB.Tables[NameTable.TableSectorsRangesRecon] as IDependentAsp)
                    .LoadByFilterAsync<TableSectorsRanges>(id);
                ucSRangesRecon.UpdateSRanges(lSRangeRecon);

                lSRangeSuppr = await (clientDB.Tables[NameTable.TableSectorsRangesSuppr] as IDependentAsp)
                    .LoadByFilterAsync<TableSectorsRanges>(id);
                ucSRangesSuppr.UpdateSRanges(lSRangeSuppr);

                lSpecFreqForbidden = await (clientDB.Tables[NameTable.TableFreqForbidden] as IDependentAsp)
                    .LoadByFilterAsync<TableFreqSpec>(id);
                ucSpecFreqForbidden.UpdateSpecFreqs(lSpecFreqForbidden);

                lSpecFreqImportant = await (clientDB.Tables[NameTable.TableFreqImportant] as IDependentAsp)
                    .LoadByFilterAsync<TableFreqSpec>(id);
                ucSpecFreqImportant.UpdateSpecFreqs(lSpecFreqImportant);

                lSpecFreqKnown = await (clientDB.Tables[NameTable.TableFreqKnown] as IDependentAsp)
                    .LoadByFilterAsync<TableFreqSpec>(id);
                ucSpecFreqKnown.UpdateSpecFreqs(lSpecFreqKnown);

                lSuppressFWS = await (clientDB.Tables[NameTable.TableSuppressFWS] as IDependentAsp)
                    .LoadByFilterAsync<TableSuppressFWS>(id);
                ucSuppressFWS.UpdateSuppressFWS(lSuppressFWS);

                lSuppressFHSS = await (clientDB.Tables[NameTable.TableSuppressFHSS] as IDependentAsp)
                    .LoadByFilterAsync<TableSuppressFHSS>(id);
                ucSuppressFHSS.UpdateSuppressFHSS(lSuppressFHSS);
            }
            catch (ExceptionClient exeptClient)
            {
                MessageBox.Show(exeptClient.Message);
            }
            catch (ExceptionDatabase excpetService)
            {
                MessageBox.Show(excpetService.Message);
            }
        }

        private async void UcTemsFWS_OnDeleteRecord(object sender, TableEvent e)
        {
            if (clientDB != null)
            {
                clientDB.Tables[e.NameTable].Delete(e.Record);
            }
            var SignalIdFWS = new int[1];
            SignalIdFWS[0] = Convert.ToInt32(e.Record.Id);
            //await dsp.StorageAction(0, 0, SignalIdFWS); // отправить запрос на удаление ИРИ ФРЧ на сервер
        }

        private async void OnClearRecords(object sender, NameTable nameTable)
        {
            if (clientDB != null)
                switch (nameTable)
                {
                    case NameTable.TempFWS:
                        var SignalsIdFWS = new int[0];
                        // dsp..StorageAction(0 - ФРЧ, 1 - ППРЧ; 0 - удалить, 1 - восстановить; массив id);
                        //await dsp.StorageAction(0, 0, SignalsIdFWS); // отправить запрос на очистку ИРИ ФРЧ на сервер
                        break;

                    case NameTable.TableReconFHSS:
                        var SignalsIdFHSS = new int[0];
                        //await VariableWork.aWPtoBearingDSPprotocolNew.StorageAction(0 - ФРЧ, 1 - ППРЧ; 0 - удалить, 1 - восстановить; массив id);
                        //await dsp.StorageAction(1, 0, SignalsIdFHSS); // отправить запрос на очистку ИРИ ППРЧ на сервер
                        break;

                    case NameTable.TableReconFWS:
                        var SignalsIdReconFWS = new int[lReconFWS.Count()];
                        for (var i = 0; i < lReconFWS.Count; i++) SignalsIdReconFWS[i] = lReconFWS[i].Id;

                        // dsp..StorageAction(0 - ФРЧ, 1 - ППРЧ; 0 - удалить, 1 - восстановить; массив id);
                        //await dsp.StorageAction(0, 1, SignalsIdReconFWS); // отправить запрос на восстановление ИРИ ФРЧ на сервер

                        clientDB.Tables[nameTable].Clear();
                        break;

                    default:
                        clientDB.Tables[nameTable].Clear();
                        break;
                }
        }

        private void OnClearRecordsByFilter(object sender, NameTable nameTable)
        {
            try
            {
                if (selectedASP > 0)
                    (clientDB.Tables[nameTable] as IDependentAsp).ClearByFilter(selectedASP);
            }
            catch (Exception)
            {
            }
        }

        private async void OnDeleteRecord(object sender, TableEvent e)
        {
            if (clientDB != null)
                switch (e.NameTable)
                {
                    case NameTable.TableReconFHSS:

                        break;

                    case NameTable.TableReconFWS:

                        clientDB.Tables[e.NameTable].Delete(e.Record);

                        break;

                    case NameTable.TableSuppressFWS:

                        clientDB.Tables[e.NameTable].Delete(e.Record);

                        break;
                    default:
                        clientDB.Tables[e.NameTable].Delete(e.Record);
                        break;
                }
        }

        private void OnChangeRecord(object sender, TableEvent e)
        {
            if (clientDB != null)
                switch (e.NameTable)
                {
                    case NameTable.TableSuppressFWS:
                        // Проверка возможности добавления в таблицу---------------------------------------------------
                        var sMessage = IsUpdateTableSuppressFWS(e.Record as TableSuppressFWS, true);
                        if (sMessage == string.Empty)
                            clientDB.Tables[e.NameTable].Change(e.Record);
                        else
                            MessageBox.Show(sMessage, SMessages.mesMessage, MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                        break;

                    case NameTable.TableFreqForbidden:
                        if (IsAddSpecFreq(lSpecFreqForbidden, e.Record as TableFreqSpec, true))
                            clientDB.Tables[e.NameTable].Change(e.Record);
                        break;

                    case NameTable.TableFreqKnown:
                        if (IsAddSpecFreq(lSpecFreqKnown, e.Record as TableFreqSpec, true))
                            clientDB.Tables[e.NameTable].Change(e.Record);
                        break;

                    case NameTable.TableFreqImportant:
                        if (IsAddSpecFreq(lSpecFreqImportant, e.Record as TableFreqSpec, true))
                            clientDB.Tables[e.NameTable].Change(e.Record);
                        break;

                    case NameTable.TableSuppressFHSS:
                        // Проверка возможности добавления в таблицу---------------------------------------------------
                        sMessage = IsUpdateTableSuppressFHSS(e.Record as TableSuppressFHSS, true);
                        if (sMessage == string.Empty)
                            clientDB.Tables[e.NameTable].Change(e.Record);
                        else
                            MessageBox.Show(sMessage, SMessages.mesMessage, MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                        // --------------------------------------------------------------------------------------------
                        break;

                    default:
                        clientDB.Tables[e.NameTable].Change(e.Record);
                        break;
                }
        }

        private void OnAddRecord(object sender, TableEvent e)
        {
            if (clientDB != null)
            {
                var sMessage = string.Empty;
                switch (e.NameTable)
                {
                    case NameTable.TableASP:
                        clientDB?.Tables[e.NameTable].Add(e.Record);

                        var buttonsNAV = new ButtonsNAV();
                        buttonsNAV.NumberASP = (e.Record as TableASP).Id;
                        buttonsNAV.IdMission = (e.Record as TableASP).IdMission;
                        clientDB?.Tables[NameTable.ButtonsNAV].Add(buttonsNAV);
                        break;

                    case NameTable.TableSuppressFWS:
                        // Проверка возможности нажатия ToggleButton РП и добавления частот в БД
                        //if (mPanel.Highlight != MainPanel.MPanel.Buttons.RadioSuppression)
                        //{
                        //    ucSuppressFWS.ToggleButtonUnchecked((e.Record as TableSuppressFWS).FreqKHz.Value);

                        //    //if (ucSuppressFWS.ToggleButtonUnchecked((e.Record as TableSuppressFWS).FreqKHz.Value))
                        //    //    return;
                        //}

                        // Проверка возможности добавления в таблицу---------------------------------------------------
                        sMessage = IsUpdateTableSuppressFWS(e.Record as TableSuppressFWS, false);
                        if (sMessage == string.Empty)
                            clientDB?.Tables[e.NameTable].Add(e.Record);
                        else
                            //   ucSuppressFWS.ToggleButtonUnchecked((e.Record as TableSuppressFWS).FreqKHz.Value);
                            MessageBox.Show(sMessage, SMessages.mesMessage, MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

                        //sMessage = IsUpdateTableSuppressFWS(e.Record as TableSuppressFWS, false);
                        //if (sMessage == string.Empty)
                        //{
                        //    clientDB?.Tables[e.NameTable].Add(e.Record);
                        //}
                        //else
                        //{
                        //    ucSuppressFWS.ToggleButtonUnchecked((e.Record as TableSuppressFWS).FreqKHz.Value);
                        //    MessageBox.Show(sMessage, "Сообщение!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        //}
                        break;

                    case NameTable.TableFreqForbidden:
                        if (IsAddSpecFreq(lSpecFreqForbidden, e.Record as TableFreqSpec, false))
                            //if (PropLocalFreqMHz_kHz.FreqMHz_kHz == 1)
                            //{
                            //    (e.Record as TableFreqSpec).FreqMinKHz = (e.Record as TableFreqSpec).FreqMinKHz * 1000;
                            //    (e.Record as TableFreqSpec).FreqMaxKHz = (e.Record as TableFreqSpec).FreqMaxKHz * 1000;
                            //}
                            clientDB?.Tables[e.NameTable].Add(e.Record);
                        break;

                    case NameTable.TableFreqKnown:
                        if (IsAddSpecFreq(lSpecFreqKnown, e.Record as TableFreqSpec, false))
                            //if (PropLocalFreqMHz_kHz.FreqMHz_kHz == 1)
                            //{
                            //    (e.Record as TableFreqSpec).FreqMinKHz = (e.Record as TableFreqSpec).FreqMinKHz * 1000;
                            //    (e.Record as TableFreqSpec).FreqMaxKHz = (e.Record as TableFreqSpec).FreqMaxKHz * 1000;
                            //}
                            clientDB?.Tables[e.NameTable].Add(e.Record);
                        break;

                    case NameTable.TableFreqImportant:
                        if (IsAddSpecFreq(lSpecFreqImportant, e.Record as TableFreqSpec, false))
                            //if(PropLocalFreqMHz_kHz.FreqMHz_kHz == 1)
                            //{
                            //    (e.Record as TableFreqSpec).FreqMinKHz = (e.Record as TableFreqSpec).FreqMinKHz * 1000;
                            //    (e.Record as TableFreqSpec).FreqMaxKHz = (e.Record as TableFreqSpec).FreqMaxKHz * 1000;
                            //}
                            clientDB?.Tables[e.NameTable].Add(e.Record);
                        break;

                    case NameTable.TableSuppressFHSS:
                        // Проверка возможности добавления в таблицу---------------------------------------------------
                        sMessage = IsUpdateTableSuppressFHSS(e.Record as TableSuppressFHSS, false);
                        if (sMessage == string.Empty)
                            clientDB?.Tables[e.NameTable].Add(e.Record);
                        else
                            MessageBox.Show(sMessage, SMessages.mesMessage, MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                        // --------------------------------------------------------------------------------------------
                        break;

                    default:
                        clientDB?.Tables[e.NameTable].Add(e.Record);
                        break;
                }
            }
        }

        private async void UcTemsFWS_OnAddFWS_RS(object sender, TableSuppressFWS e)
        {
            if (PropNumberASP.SelectedNumASP > 0)
            {
                e.NumberASP = PropNumberASP.SelectedNumASP;

                // Проверка возможности добавления в таблицу---------------------------------------------------
                var sMessage = IsUpdateTableSuppressFWS(e, false);
                if (sMessage == string.Empty)
                    clientDB.Tables[NameTable.TableSuppressFWS].Add(e);
                else
                    MessageBox.Show(sMessage, SMessages.mesMessage, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                // --------------------------------------------------------------------------------------------
                clientDB.Tables[NameTable.TableSuppressFWS].Add(e);
            }
        }

        private async void UcTemsFWS_OnAddFWS_TD(object sender, TableReconFWS e)
        {
            if (PropNumberASP.SelectedNumASP > 0) clientDB.Tables[NameTable.TableReconFWS].Add(e);
        }

        private async void UcTemsFWS_OnGetExecBear(object sender, TempFWS e)
        {
            var bearing = await ExternalExBearing2(e.FreqKHz / 1000d, e.Deviation / 1000d);
            if (bearing != -1)
            {
                var ind = e.ListQ.ToList().FindIndex(x => x.NumberASP == PropNumberASP.SelectedASPforListQ);
                if (ind != -1)
                {
                    e.ListQ[ind].Bearing = bearing;
                    clientDB.Tables[NameTable.TempFWS].Change(e);
                }
            }
        }

        private async void UcTemsFWS_OnGetKvBear(object sender, TempFWS e)
        {
            var PhAvCount = 3;
            var PlAvCount = 3;

            PhAvCount = Properties.Global.NumberAveragingPhase <= 0
                ? PhAvCount
                : Properties.Global.NumberAveragingPhase;
            PlAvCount = Properties.Global.NumberAveragingBearing <= 0
                ? PlAvCount
                : Properties.Global.NumberAveragingBearing;
        }

        private void UcTemsFWS_OnSendFreqCRRD(object sender, TempFWS e)
        {
            //AroneConnetion.FrequencyFromPanorama(e.FreqKHz / 1000d);

            SendFreqToCRRD1(e.FreqKHz / 1000d);
        }

        private void UcTemsFWS_OnSendFreqCRRD2(object sender, TempFWS e)
        {
            //AroneConnetion.FrequencyFromPanorama(e.FreqKHz / 1000d);

            SendFreqToCRRD2(e.FreqKHz / 1000d);
        }

        private void UcTemsFWS_OnSelectedRow(object sender, TableEvent e)
        {
            clientDB.Tables[e.NameTable].Change(e.Record);

            //if (AutoToggleButton.IsChecked.Value)
            //{
            //    TempFWS tempFWS = (TempFWS)e.Record;
            //    pLibrary.CenterFreq(tempFWS.FreqKHz / 1000d, 30);
            //    AroneConnetion.FrequencyFromPanorama(tempFWS.FreqKHz / 1000d);
            //    ControlAR6000First.FrequencyFromPanorama(tempFWS.FreqKHz / 1000d);
            //    ControlArOneSec.FrequencyFromPanorama(tempFWS.FreqKHz / 1000d);
            //    ControlAR6000Sec.FrequencyFromPanorama(tempFWS.FreqKHz / 1000d);
            //}
        }

        private void UcSRangesSuppr_OnLoadDefaultSRanges(object sender, NameTable e)
        {
            var sr = new List<TableSectorsRanges>();
            var Ranges = new object();
            if (e == NameTable.TableSectorsRangesSuppr)
            {
                sr = ucSRangesSuppr.LoadDefaultSRangesSuppr(Properties.Local.Common.Language);
                if (sr.Count > 0)
                    for (var i = 0; i < sr.Count; i++)
                    {
                        sr[i].IsCheck = true;
                        sr[i].NumberASP = PropNumberASP.SelectedNumASP;
                    }

                Ranges = (from t in sr let c = t.ToRangesSuppr() select c).ToList();
            }

            clientDB.Tables[e].AddRange(Ranges);

        }

        private void UcSRangesRecon_OnLoadDefaultSRanges(object sender, NameTable e)
        {
            var sr = new List<TableSectorsRanges>();
            var Ranges = new object();
            if (e == NameTable.TableSectorsRangesRecon)
            {
                sr = ucSRangesRecon.LoadDefaultSRangesRecon(Properties.Local.Common.Language);
                if (sr.Count > 0)
                    for (var i = 0; i < sr.Count; i++)
                    {
                        sr[i].IsCheck = true;
                        sr[i].NumberASP = PropNumberASP.SelectedNumASP;
                    }

                Ranges = (from t in sr let c = t.ToRangesRecon() select c).ToList();
            }

            clientDB.Tables[e].AddRange(Ranges);
        }

        private void UcSuppressFWS_OnDeleteRange(object sender, List<TableSuppressFWS> e)
        {
            try
            {
                if (clientDB != null)
                    clientDB.Tables[NameTable.TableSuppressFWS].RemoveRange(e);
            }
            catch
            {
            }
        }

        private void UcSuppressFWS_OnAddRange(object sender, List<TableSuppressFWS> e)
        {
            try
            {
                if (clientDB != null)
                    clientDB.Tables[NameTable.TableSuppressFWS].AddRange(e);
            }
            catch
            {
            }
        }

        private void UcSuppressFWS_OnSendFreqCRRD(object sender, TempFWS e)
        {
            //AroneConnetion.FrequencyFromPanorama(e.FreqKHz / 1000d);

            SendFreqToCRRD1(e.FreqKHz / 1000d);
        }

        private void UcSuppressFWS_OnSendFreqCRRD2(object sender, TempFWS e)
        {
            SendFreqToCRRD2(e.FreqKHz / 1000d);
        }

        private async void UcSuppressFWS_OnGetExecBear(object sender, TableSuppressFWS e)
        {
            var bearing = await ExternalExBearing2(e.FreqKHz.Value / 1000d, 0);
            //bearing = 15f;
            e.Bearing = bearing;
            clientDB.Tables[NameTable.TableSuppressFWS].Change(e);
        }

        public async Task<float> ExternalExBearing2(double freqMHz, double freqWidthMHz)
        {
            if (freqWidthMHz <= 0) freqWidthMHz = 0.0036;

            var MinBandXMHz = freqMHz - freqWidthMHz;
            var MaxBandXMHz = freqMHz + freqWidthMHz;

            var PhAvCount = Properties.Global.NumberAveragingPhase <= 0 ? 3 : Properties.Global.NumberAveragingPhase;
            var PlAvCount = Properties.Global.NumberAveragingBearing <= 0
                ? 3
                : Properties.Global.NumberAveragingBearing;

            //var answer = await dsp.ExecutiveDF(MinBandXMHz, MaxBandXMHz, PhAvCount, PlAvCount);

            float bearing = -1;
            //if (answer?.Header.ErrorCode == 0)
            //{
            //    PTabControl.SelectedIndex = 2;
            //    pLibrary.IQExBearingPaited(answer.Direction, answer.CorrelationHistogram, answer.Frequency, answer.StandardDeviation, answer.DiscardedDirectionPercent);
            //    bearing = answer.Direction / 10f;
            //}
            return bearing;
        }

        private void UcReconFHSS_OnSelectedRow(object sender, TableEvent e)
        {
            clientDB.Tables[e.NameTable].Change(e.Record);
        }

        private void UcReconFHSS_OnAddFHSS_RS_Recon(object sender, TableSuppressFHSS e)
        {
            // Проверка возможности добавления в таблицу---------------------------------------------------
            var sMessage = IsUpdateTableSuppressFHSS(e, false);
            if (sMessage == string.Empty)
                clientDB.Tables[NameTable.TableSuppressFHSS].Add(e);
            else
                MessageBox.Show(sMessage, SMessages.mesMessage, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            // --------------------------------------------------------------------------------------------
        }

        private void OnAddTableToReport(object sender, TableEventReport e)
        {
            switch (Properties.Local.Common.FileType)
            {
                case FileType.Word:
                    AddTableToReport.AddToWord("First", e.Table, e.NameTable);
                    break;

                case FileType.Excel:
                    AddTableToReport.AddToExcel("First", e.Table, e.NameTable);
                    break;

                case FileType.Txt:
                    AddTableToReport.AddToTxt("First", e.Table, e.NameTable);
                    break;
            }
        }
    }
}