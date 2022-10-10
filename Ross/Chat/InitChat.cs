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
            Events.OnDoActionWithMessage += Events_OnDoActionWithMessage;
        }

        private void Events_OnDoActionWithMessage(List<Message> stationsMessages)
        {
            foreach(var stationModel in SelectedStationModels)
            {
                foreach (var mesage in stationsMessages)
                {
                    if(mesage.Id == stationModel.IdMaster || mesage.Id == stationModel.IdSlave)
                    {
                        stationModel.SelectedConnectionObject.SendTextMessage(mesage.MessageFiled);
                    }
                }
            }
            
        }

        private void UpdateSideMenu(List<TableASP> ASPList)
        {
            newWindow.UpdateSideMenu(ASPList);
        }


       


    }
}
