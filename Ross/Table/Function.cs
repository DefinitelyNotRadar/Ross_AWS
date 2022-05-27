using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ModelsTablesDBLib;
using TableEvents;
using ValuesCorrectLib;

namespace Ross
{
    public partial class MainWindow
    {
        private ObservableCollection<ObservableCollection<string>> lASPRP =
            new ObservableCollection<ObservableCollection<string>>();

        /// <summary>
        ///     Обновить поле АСП РП в таблице
        /// </summary>
        /// <param name="tableASP"></param>
        /// <returns></returns>
        private ObservableCollection<string> UpdateASPRPRecon(List<TableASP> tableASP)
        {
            var lASPRP = new ObservableCollection<string>();

            for (var i = 0; i < tableASP.Count; i++)
                if (tableASP[i].ISOwn)
                    lASPRP.Add("*" + tableASP[i].Id);
                else
                    lASPRP.Add(tableASP[i].Id.ToString());
            return lASPRP;
        }

        /// <summary>
        ///     Проверка возможности добавления диапазона в таблицу
        /// </summary>
        /// <param name="lTemp"> Текущий список диапазонов </param>
        /// <param name="recAdd"> Диапазон для добавления в таблицу </param>
        /// <param name="IsChange">
        ///     true - проверка на возможность изменения записи,
        ///     false - проверка на возможность добавления записи
        /// </param>
        /// <returns> true - диапазон можно добавить </returns>
        private bool IsAddSpecFreq(List<TableFreqSpec> lTemp, TableFreqSpec recAdd, bool IsChange)
        {
            var tempListSpecFreq = new List<TableFreqSpec>();
            if (IsChange)
                tempListSpecFreq = lTemp.Where(rec => rec.Id != recAdd.Id).ToList();
            else
                tempListSpecFreq = lTemp;

            CorrectMinMax.IsCorrectMinMax(recAdd, PropNameTable.SpecFreqsName);
            if (CorrectMinMax.IsCorrectRangeMinMax(recAdd))
            {
                var tempSpecFreq = new ObservableCollection<TableFreqSpec>(tempListSpecFreq);
                var addSpecFreq = new ObservableCollection<TableFreqSpec>();
                addSpecFreq.Add(recAdd);
                if (CorrectSpecFreq.IsAddRecordsToCollection(tempSpecFreq, addSpecFreq)) return true;
            }

            return false;
        }

        /// <summary>
        ///     Добавление/Изменение в TableSuppressFWS
        /// </summary>
        /// <param name="SuppressFWS"> модель ИРИ для РП </param>
        /// <returns> сообщение об ошибке (пустая строка - успешно) </returns>
        public string IsUpdateTableSuppressFWS(TableSuppressFWS SuppressFWS, bool IsChange)
        {
            var tempListSuppressFWS = new List<TableSuppressFWS>();
            if (IsChange)
                tempListSuppressFWS = lSuppressFWS.Where(rec => rec.Id != SuppressFWS.Id).ToList();
            else
                tempListSuppressFWS = lSuppressFWS;

            var sMessage = CorrectSuppress.IsCheckRangesSuppress(SuppressFWS.FreqKHz.Value, lSRangeSuppr);
            if (sMessage != string.Empty)
                return sMessage;

            sMessage = CorrectSuppress.IsCheckForbiddenFreq(SuppressFWS.FreqKHz.Value, lSpecFreqForbidden);
            if (sMessage != string.Empty)
                return sMessage;

            sMessage = CorrectSuppress.CountFreqLetter(SuppressFWS.Letter.Value, Properties.Global.NumberIri,
                tempListSuppressFWS);
            if (sMessage != string.Empty)
                return sMessage;

            return sMessage;
        }

        /// <summary>
        /// </summary>
        /// <param name="SuppressFHSS"></param>
        /// <param name="IsChange">
        ///     true - проверка на возможность изменения записи,
        ///     false - проверка на возможность добавления записи
        /// </param>
        /// <returns></returns>
        public string IsUpdateTableSuppressFHSS(TableSuppressFHSS SuppressFHSS, bool IsChange)
        {
            var sMessage =
                CorrectSuppress.IsCheckRangesSuppress(SuppressFHSS.FreqMinKHz, SuppressFHSS.FreqMaxKHz, lSRangeSuppr);
            if (sMessage != string.Empty)
                return sMessage;

            sMessage = CorrectSuppress.IsCheckForbiddenRange(SuppressFHSS.FreqMinKHz, SuppressFHSS.FreqMaxKHz,
                lSpecFreqForbidden.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList());
            if (sMessage != string.Empty)
                return sMessage;

            var tempListSuppressFHSS = new List<TableSuppressFHSS>();
            if (IsChange)
                tempListSuppressFHSS = lSuppressFHSS.Where(rec => rec.Id != SuppressFHSS.Id).ToList();
            else
                tempListSuppressFHSS = lSuppressFHSS;

            // Проверка возможности добавления записи ------------------------------------------------------------------------------------
            if (tempListSuppressFHSS.Count == 4)
            {
                sMessage = SMessages.mesUnableAddRecord;
            }
            else
            {
                if (!CorrectSuppress.IsCheckCountChannel(tempListSuppressFHSS, SuppressFHSS))
                    sMessage = SMessages.mesCountChannelsMax;
            }

            //if (lSuppressFHSS.Count == 4)
            //{
            //    sMessage = "Невозможно добавить запись! Количество записей превысит допустимое!";
            //}
            //else
            //{
            //    if (!CorrectSuppress.IsCheckCountChannel(lSuppressFHSS, SuppressFHSS))
            //    {
            //        sMessage = "Суммарное количество каналов превышает допустимое!";
            //    }
            //}
            //----------------------------------------------------------------------------------------------------------------------------
            return sMessage;
        }

        /// <summary>
        ///     Убрать Известные частоты из списка
        /// </summary>
        /// <param name="listTempFWS"></param>
        /// <returns></returns>
        private List<TempFWS> ExcludeKnownFreq(List<TempFWS> listTempFWS)
        {
            var tempFreqKnown = lSpecFreqKnown.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
            var listExclude = new List<TempFWS>(listTempFWS);
            var f = false;

            if (listTempFWS.Count > 0 && tempFreqKnown.Count > 0)
            {
                for (var i = 0; i < listTempFWS.Count; i++)
                {
                    for (var j = 0; j < tempFreqKnown.Count; j++)
                        if (listTempFWS[i].FreqKHz >= tempFreqKnown[j].FreqMinKHz &&
                            listTempFWS[i].FreqKHz <= tempFreqKnown[j].FreqMaxKHz)
                            f = true;
                    if (f)
                    {
                        listExclude.Remove(listTempFWS[i]);
                        f = false;
                    }
                }

                return listExclude;
            }

            return listTempFWS;
        }

        private List<TempFWS> ExcludeKnownForbiddenFreqs(List<TempFWS> listTempFWS)
        {
            var tempFreqKnown = lSpecFreqKnown.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
            var tempFreqForbidden = lSpecFreqForbidden.Where(x => x.NumberASP == PropNumberASP.SelectedNumASP).ToList();
            var listExcludeKnown = new List<TempFWS>(listTempFWS);
            var listExcludeForbidden = new List<TempFWS>();

            if (listTempFWS.Count > 0)
            {
                if (tempFreqKnown.Count > 0)
                    // Исключить Известные частоты
                    for (var i = 0; i < listTempFWS.Count; i++)
                    for (var j = 0; j < tempFreqKnown.Count; j++)
                        if (listTempFWS[i].FreqKHz >= tempFreqKnown[j].FreqMinKHz &&
                            listTempFWS[i].FreqKHz <= tempFreqKnown[j].FreqMaxKHz)
                            listExcludeKnown.Remove(listTempFWS[i]);

                listExcludeForbidden = new List<TempFWS>(listExcludeKnown);

                if (tempFreqForbidden.Count > 0)
                    // Исключить Запрещенные частоты
                    for (var i = 0; i < listExcludeForbidden.Count; i++)
                    for (var j = 0; j < tempFreqForbidden.Count; j++)
                        if (listExcludeKnown[i].FreqKHz >= tempFreqForbidden[j].FreqMinKHz &&
                            listExcludeKnown[i].FreqKHz <= tempFreqForbidden[j].FreqMaxKHz)
                            listExcludeForbidden.Remove(listExcludeKnown[i]);

                return listExcludeForbidden;
            }

            return listTempFWS;
        }

        private void UpdateRanges(GlobalProperties globalProperties)
        {
            PropGlobalProperty.RangeRadioRecon = (byte)globalProperties.RangeRadioRecon;
            PropGlobalProperty.RangeJamming = (byte)globalProperties.RangeJamming;
            ucSuppressFWS.ToggleButtonVisible();
        }

        private void UpdateFrequencyUnits(LocalProperties localProperties)
        {
            //PropLocalFreqMHz_kHz.FreqMHz_kHz = (byte)localProperties.Common.FrequencyUnits;

            ucSRangesRecon.UpdateSRanges(lSRangeRecon);
            ucSRangesSuppr.UpdateSRanges(lSRangeSuppr);

            ucSpecFreqKnown.UpdateSpecFreqs(lSpecFreqKnown);
            ucSpecFreqForbidden.UpdateSpecFreqs(lSpecFreqForbidden);
            ucSpecFreqImportant.UpdateSpecFreqs(lSpecFreqImportant);


            ucReconFHSS.UpdateReconFHSS(lReconFHSS);

            ucSuppressFWS.UpdateSuppressFWS(lSuppressFWS);
            ucSuppressFHSS.UpdateSuppressFHSS(lSuppressFHSS);
            ucSuppressFHSS.UpdateFHSSExcludedFreq(lFHSSExcludedFreq);

            ChangeFreqHeaderMHz_kHz();
        }

        private void ChangeFreqHeaderMHz_kHz()
        {
            //ucSRangesRecon.ChangeFreqHeaderMHz_kHz();
            //ucSRangesSuppr.ChangeFreqHeaderMHz_kHz();
            //ucSpecFreqKnown.ChangeFreqHeaderMHz_kHz();
            //ucSpecFreqForbidden.ChangeFreqHeaderMHz_kHz();
            //ucSpecFreqImportant.ChangeFreqHeaderMHz_kHz();
            //ucTemsFWS.ChangeFreqHeaderMHz_kHz();
            //ucReconFWS.ChangeFreqHeaderMHz_kHz();
            //ucReconFHSS.ChangeFreqHeaderMHz_kHz();
            //ucSuppressFWS.ChangeFreqHeaderMHz_kHz();
            //ucSuppressFHSS.ChangeFreqHeaderMHz_kHz();
        }

        private void UpdateButtonsCRRD(LocalProperties localProperties)
        {
            PropLocalCRRD.CRRD1 = localProperties.ARONE1.State;
            PropLocalCRRD.CRRD2 = localProperties.ARONE2.State;

            ucTempFWS.ButtonsCRRDVisible();
            //ucReconFWS.ButtonsCRRDVisible();
            ucSuppressFWS.ButtonsCRRDVisible();
        }

        private void SendFreqToCRRD1(double dFreq)
        {
            //var type = Properties.Local.ARONE1.Type.ToString();

            //switch (type)
            //{
            //    case "AR_6000":
            //        ControlAR6000First.FrequencyFromPanorama(dFreq);
            //        break;
            //    case "AR_ONE":
            //        AroneConnetion.FrequencyFromPanorama(dFreq);
            //        break;
            //    default:
            //        break;
            //}
        }

        private void SendFreqToCRRD2(double dFreq)
        {
            //var type = Properties.Local.ARONE2.Type.ToString();

            //switch (type)
            //{
            //    case "AR_6000":
            //        ControlAR6000Sec.FrequencyFromPanorama(dFreq);
            //        break;
            //    case "AR_ONE":
            //        ControlArOneSec.FrequencyFromPanorama(dFreq);
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}