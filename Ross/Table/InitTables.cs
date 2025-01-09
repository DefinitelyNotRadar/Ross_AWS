using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using TableEvents;

namespace Ross
{
    public partial class MainWindow
    {
        public void InitTables()
        {
            // Таблица АСП
            ucASP.OnAddRecord += OnAddRecord;
            ucASP.OnChangeRecord += OnChangeRecord;
            ucASP.OnDeleteRecord += OnDeleteRecord;
            ucASP.OnClearRecords += OnClearRecords;
            ucASP.OnSelectedRow += UcASP_OnSelectedRow;
            ucASP.OnIsWindowPropertyOpen += UcASP_OnIsWindowPropertyOpen;
            ucASP.OnAddTableToReport += OnAddTableToReport;


            // Таблица Запрещенныее частоты
            ucSpecFreqForbidden.OnAddRecord += OnAddRecord;
            ucSpecFreqForbidden.OnChangeRecord += OnChangeRecord;
            ucSpecFreqForbidden.OnDeleteRecord += OnDeleteRecord;
            //ucSpecFreqForbidden.OnClearRecords += OnClearRecords;            
            ucSpecFreqForbidden.OnClearRecords += OnClearRecordsByFilter;
            ucSpecFreqForbidden.OnIsWindowPropertyOpen += UcSpecFreqForbidden_OnIsWindowPropertyOpen;
            ucSpecFreqForbidden.OnAddTableToReport += OnAddTableToReport;

            // Таблица Важные частоты
            ucSpecFreqImportant.OnAddRecord += OnAddRecord;
            ucSpecFreqImportant.OnChangeRecord += OnChangeRecord;
            ucSpecFreqImportant.OnDeleteRecord += OnDeleteRecord;
            //ucSpecFreqImportant.OnClearRecords += OnClearRecords;
            ucSpecFreqImportant.OnClearRecords += OnClearRecordsByFilter;
            ucSpecFreqImportant.OnIsWindowPropertyOpen += UcSpecFreqImportant_OnIsWindowPropertyOpen;
            ucSpecFreqImportant.OnAddTableToReport += OnAddTableToReport;

            // Таблица Известные частоты
            ucSpecFreqKnown.OnAddRecord += OnAddRecord;
            ucSpecFreqKnown.OnChangeRecord += OnChangeRecord;
            ucSpecFreqKnown.OnDeleteRecord += OnDeleteRecord;
            //ucSpecFreqKnown.OnClearRecords += OnClearRecords;
            ucSpecFreqKnown.OnClearRecords += OnClearRecordsByFilter;
            ucSpecFreqKnown.OnIsWindowPropertyOpen += UcSpecFreqKnown_OnIsWindowPropertyOpen;
            ucSpecFreqKnown.OnAddTableToReport += OnAddTableToReport;

            // Таблица Сектора и диапазоны РР
            ucSRangesRecon.OnAddRecord += OnAddRecord;
            ucSRangesRecon.OnChangeRecord += OnChangeRecord;
            ucSRangesRecon.OnDeleteRecord += OnDeleteRecord;
            //ucSRangesRecon.OnClearRecords += OnClearRecords;
            ucSRangesRecon.OnClearRecords += OnClearRecordsByFilter;
            ucSRangesRecon.OnLoadDefaultSRanges += UcSRangesRecon_OnLoadDefaultSRanges;
            ucSRangesRecon.OnIsWindowPropertyOpen += UcSRangesRecon_OnIsWindowPropertyOpen;
            ucSRangesRecon.OnAddTableToReport += OnAddTableToReport;

            // Таблица Сектора и диапазоны РП
            ucSRangesSuppr.OnAddRecord += OnAddRecord;
            ucSRangesSuppr.OnChangeRecord += OnChangeRecord;
            ucSRangesSuppr.OnDeleteRecord += OnDeleteRecord;
            //ucSRangesSuppr.OnClearRecords += OnClearRecords;
            ucSRangesSuppr.OnClearRecords += OnClearRecordsByFilter;
            ucSRangesSuppr.OnLoadDefaultSRanges += UcSRangesSuppr_OnLoadDefaultSRanges;
            ucSRangesSuppr.OnIsWindowPropertyOpen += UcSRangesSuppr_OnIsWindowPropertyOpen;
            ucSRangesSuppr.OnAddTableToReport += OnAddTableToReport;

            // Таблица ИРИ ФРЧ ЦР
            ucReconFWS.OnDeleteRecord += new EventHandler<TableEvent>(OnDeleteRecord);
            ucReconFWS.OnClearRecords += new EventHandler<NameTable>(OnClearRecords);
            ucReconFWS.OnAddFWS_RS += new EventHandler<TableSuppressFWS>(UcReconFWS_OnAddFWS_RS);
            ucReconFWS.OnGetExecBear += new EventHandler<TableReconFWS>(UcReconFWS_OnGetExecBear);
            ucReconFWS.OnGetKvBear += new EventHandler<TableReconFWS>(UcReconFWS_OnGetKvBear);
            ucReconFWS.OnSendFreqCRRD += new EventHandler<TempFWS>(UcReconFWS_OnSendFreqCRRD);
            ucReconFWS.OnSendFreqCRRD2 += new EventHandler<TempFWS>(UcReconFWS_OnSendFreqCRRD2);
            ucReconFWS.OnClickTDistribution += new EventHandler(UcReconFWS_OnClickTDistribution);
            ucReconFWS.OnSendFWS_TD_RS += new EventHandler<List<TableSuppressFWS>>(UcReconFWS_OnSendFWS_TD_RS);
            ucReconFWS.OnClickUS += new EventHandler<bool>(UcReconFWS_OnClickUS);
            ucReconFWS.OnClickRS += new EventHandler<bool>(UcReconFWS_OnClickRS);
            ucReconFWS.OnAddTableToReport += new EventHandler<TableEventReport>(OnAddTableToReport);
            ucReconFWS.OnSelectedASPSuppr += new EventHandler<TableReconFWS>(UcReconFWS_OnSelectedASPSuppr);
            ucReconFWS.OnItemSelected += new EventHandler<TableReconFWS>(UcReconFWS_OnSelectedItem);
            ucReconFWS.ClearReconFWS(); 
           

            // Таблица ИРИ ФРЧ РП
            ucSuppressFWS.OnAddRecord += OnAddRecord;
            ucSuppressFWS.OnChangeRecord += OnChangeRecord;
            ucSuppressFWS.OnDeleteRecord += OnDeleteRecord;
            //ucSuppressFWS.OnClearRecords += OnClearRecords;
            ucSuppressFWS.OnClearRecords += OnClearRecordsByFilter;
            ucSuppressFWS.OnDeleteRange += UcSuppressFWS_OnDeleteRange;
            ucSuppressFWS.OnAddRange += UcSuppressFWS_OnAddRange;
            ucSuppressFWS.OnGetExecBear += UcSuppressFWS_OnGetExecBear;
            ucSuppressFWS.OnSendFreqCRRD += UcSuppressFWS_OnSendFreqCRRD;
            ucSuppressFWS.OnSendFreqCRRD2 += UcSuppressFWS_OnSendFreqCRRD2;
            ucSuppressFWS.OnIsWindowPropertyOpen += UcSuppressFWS_OnIsWindowPropertyOpen;
            ucSuppressFWS.OnAddTableToReport += OnAddTableToReport;

            // Таблица ИРИ ППРЧ
            ucReconFHSS.OnDeleteRecord += OnDeleteRecord;
            ucReconFHSS.OnClearRecords += OnClearRecords;
            ucReconFHSS.OnSelectedRow += UcReconFHSS_OnSelectedRow;
            ucReconFHSS.OnAddFHSS_RS_Recon += UcReconFHSS_OnAddFHSS_RS_Recon;
            ucReconFHSS.OnAddTableToReport += OnAddTableToReport;

            // Таблица ИРИ ППРЧ РП
            ucSuppressFHSS.OnAddRecord += OnAddRecord;
            ucSuppressFHSS.OnChangeRecord += OnChangeRecord;
            ucSuppressFHSS.OnDeleteRecord += OnDeleteRecord;
            ucSuppressFHSS.OnClearRecords += OnClearRecords;
            ucSuppressFHSS.OnIsWindowPropertyOpen += UcSuppressFHSS_OnIsWindowPropertyOpen;
            ucSuppressFHSS.OnIsWindowPropertyOpenExc += UcSuppressFHSS_OnIsWindowPropertyOpenExc;
            ucSuppressFHSS.OnAddTableToReport += OnAddTableToReport;
        }


    }
}