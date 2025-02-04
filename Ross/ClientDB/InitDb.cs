﻿using ClientDataBase;
using InheritorsEventArgs;
using ModelsTablesDBLib;
using System;
using System.Threading.Tasks;

namespace Ross
{
    public partial class MainWindow
    {
        private string endPoint = "127.0.0.1:8302";
        private ClientDB clientDB;


        //TODO: по номеру арма из настроек брать
        private new string Name { get; } = "ROSS";

        private void InitClientDBAsync()
        {
            clientDB.OnConnect += HandlerConnect_ClientDb;
            clientDB.OnDisconnect += HandlerDisconnect_ClientDb;
            clientDB.OnErrorDataBase += HandlerError_ClientDb;
            (clientDB.Tables[NameTable.TableASP] as ITableUpdate<TableASP>).OnUpTable += HandlerUpdate_TableASP;
            (clientDB.Tables[NameTable.TableSectorsRangesRecon] as ITableUpdate<TableSectorsRangesRecon>).OnUpTable += HandlerUpdate_TableSectorRangesRecon;
            (clientDB.Tables[NameTable.TableSectorsRangesSuppr] as ITableUpdate<TableSectorsRangesSuppr>).OnUpTable += HandlerUpdate_TableSectorRangesSuppr;
            (clientDB.Tables[NameTable.TableFreqForbidden] as ITableUpdate<TableFreqForbidden>).OnUpTable += HandlerUpdate_TableFreqForbidden;
            (clientDB.Tables[NameTable.TableFreqImportant] as ITableUpdate<TableFreqImportant>).OnUpTable += HandlerUpdate_TableFreqImportant;
            (clientDB.Tables[NameTable.TableFreqKnown] as ITableUpdate<TableFreqKnown>).OnUpTable += HandlerUpdate_TableFreqKnown;
            (clientDB.Tables[NameTable.GlobalProperties] as ITableUpdate<GlobalProperties>).OnUpTable += HandlerUpdate_GlobalProperties;
            (clientDB.Tables[NameTable.TableSuppressFWS] as ITableUpdate<TableSuppressFWS>).OnUpTable += HandlerUpdate_TableSuppressFWS;
            (clientDB.Tables[NameTable.TempSuppressFWS] as ITableUpdate<TempSuppressFWS>).OnUpTable += HandlerUpdate_TempSuppressFWS;
            (clientDB.Tables[NameTable.TableSuppressFHSS] as ITableUpdate<TableSuppressFHSS>).OnUpTable += HandlerUpdate_TableSuppressFHSS;
            (clientDB.Tables[NameTable.TableFHSSExcludedFreq] as ITableUpdate<TableFHSSExcludedFreq>).OnUpTable += HandlerUpdate_TableFHSSExcludedFreq;
            (clientDB.Tables[NameTable.TableReconFHSS] as ITableUpdate<TableReconFHSS>).OnUpTable += HandlerUpdate_TableReconFHSS;            
            (clientDB.Tables[NameTable.TempGNSS] as ITableUpdate<TempGNSS>).OnUpTable +=  HandlerUpdate_TempGNSS;
            (clientDB.Tables[NameTable.TableReconFWS] as ITableUpdate<TableReconFWS>).OnUpTable += HandlerUpdate_TableReconFWS;
            (clientDB.Tables[NameTable.TableReconFWS] as ITableAddRange<TableReconFWS>).OnAddRange += HandlerAddRangeReconFWS;
            (clientDB.Tables[NameTable.TableReconFHSS] as ITableAddRange<TableReconFHSS>).OnAddRange += HandlerAddRangeReconFHSS;
            (clientDB.Tables[NameTable.TableChat] as ITableUpdate<TableChatMessage>).OnUpTable += HandlerUpdate_TableChat;
            (clientDB.Tables[NameTable.TableRoute] as ITableUpdate<TableRoute>).OnUpTable += HandlerUpdate_TableRoute;

        }

        private void DeinitClientDB()
        {
            clientDB.OnConnect -= HandlerConnect_ClientDb;
            clientDB.OnDisconnect -= HandlerDisconnect_ClientDb;
            clientDB.OnErrorDataBase -= HandlerError_ClientDb;
            (clientDB.Tables[NameTable.TableASP] as ITableUpdate<TableASP>).OnUpTable -= HandlerUpdate_TableASP;
            (clientDB.Tables[NameTable.TableSectorsRangesRecon] as ITableUpdate<TableSectorsRangesRecon>).OnUpTable -= HandlerUpdate_TableSectorRangesRecon;
            (clientDB.Tables[NameTable.TableSectorsRangesSuppr] as ITableUpdate<TableSectorsRangesSuppr>).OnUpTable -= HandlerUpdate_TableSectorRangesSuppr;
            (clientDB.Tables[NameTable.TableFreqForbidden] as ITableUpdate<TableFreqForbidden>).OnUpTable -= HandlerUpdate_TableFreqForbidden;
            (clientDB.Tables[NameTable.TableFreqImportant] as ITableUpdate<TableFreqImportant>).OnUpTable -= HandlerUpdate_TableFreqImportant;
            (clientDB.Tables[NameTable.TableFreqKnown] as ITableUpdate<TableFreqKnown>).OnUpTable -= HandlerUpdate_TableFreqKnown;
            (clientDB.Tables[NameTable.GlobalProperties] as ITableUpdate<GlobalProperties>).OnUpTable -= HandlerUpdate_GlobalProperties;
            (clientDB.Tables[NameTable.TableSuppressFWS] as ITableUpdate<TableSuppressFWS>).OnUpTable -= HandlerUpdate_TableSuppressFWS;
            (clientDB.Tables[NameTable.TempSuppressFWS] as ITableUpdate<TempSuppressFWS>).OnUpTable -= HandlerUpdate_TempSuppressFWS;
            (clientDB.Tables[NameTable.TableSuppressFHSS] as ITableUpdate<TableSuppressFHSS>).OnUpTable -= HandlerUpdate_TableSuppressFHSS;
            (clientDB.Tables[NameTable.TableFHSSExcludedFreq] as ITableUpdate<TableFHSSExcludedFreq>).OnUpTable -= HandlerUpdate_TableFHSSExcludedFreq;
            (clientDB.Tables[NameTable.TableReconFHSS] as ITableUpdate<TableReconFHSS>).OnUpTable -= HandlerUpdate_TableReconFHSS;
            (clientDB.Tables[NameTable.TempGNSS] as ITableUpdate<TempGNSS>).OnUpTable -= HandlerUpdate_TempGNSS;
            (clientDB.Tables[NameTable.TableReconFWS] as ITableUpdate<TableReconFWS>).OnUpTable -= HandlerUpdate_TableReconFWS;
            (clientDB.Tables[NameTable.TableReconFWS] as ITableAddRange<TableReconFWS>).OnAddRange -= HandlerAddRangeReconFWS;
            (clientDB.Tables[NameTable.TableReconFHSS] as ITableAddRange<TableReconFHSS>).OnAddRange -= HandlerAddRangeReconFHSS;
            (clientDB.Tables[NameTable.TableChat] as ITableUpdate<TableChatMessage>).OnUpTable -= HandlerUpdate_TableChat;
            (clientDB.Tables[NameTable.TableRoute] as ITableUpdate<TableRoute>).OnUpTable -= HandlerUpdate_TableRoute;
        }

    }
}