using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.Windows;
using UserControl_Chat;

namespace Ross
{
    public partial class MainWindow
    {
        private Chat newWindow;
        private Buble chatBuble;

        private void InitChat()
        {
            newWindow = new Chat();
            chatBuble = new Buble();
            newWindow.SetStations();
        }

        private void UpdateSideMenu(List<TableASP> ASPList)
        {
            newWindow.UpdateSideMenu(ASPList);
        }


       


    }
}
