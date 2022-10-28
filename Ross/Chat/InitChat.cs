using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.Windows;
using UserControl_Chat;

namespace Ross
{
    using System.Linq;
    using System.Threading;

    public partial class MainWindow
    {
        private Chat newWindow;
        private Buble chatBuble;

        private void InitChat()
        {
            newWindow = new Chat();
            chatBuble = new Buble();
            newWindow.SetStations();
            //Events.OnDoActionWithMessage += Events_OnDoActionWithMessage;
            newWindow.OnReturnApprovedMessages += NewWindow_OnReturnApprovedMessages;
        }

        private void NewWindow_OnReturnApprovedMessages(object sender, List<Message> messages)
        {
            try
            {
                var lastMessage = messages.Last();
                ////TODO: not 255
                //if (messages.Last().Id == 255)
                //{
                //Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                //    {
                        Cliant_SendMessage(lastMessage.MessageFiled, lastMessage.Id); //TODO: check
                        //  return;
                        //}
                        var result = SelectedStationModels.FirstOrDefault(t => t.IdMaster == lastMessage.Id)?.SelectedConnectionObject?.SendTextMessage(lastMessage.MessageFiled);

                        if (result == true)
                        {
                            Client_ConfirmLastMessage(lastMessage.Id);
                        }
                    //});
                
                //if (mesage.Id == stationModel.IdMaster || mesage.Id == stationModel.IdSlave)
                //{
                //    stationModel.SelectedConnectionObject.SendTextMessage(mesage.MessageFiled);
                //}         
            }
            catch { }
        }



        public void Cliant_SendMessage(object data, int receiver)
        {
            //TODO: исправить 255 на универсальное
            var message = new TableChatMessage() { SenderAddress = clientAddress, ReceiverAddress = receiver, Time = DateTime.Now, Status = ChatMessageStatus.Sent, Text = data as string };
            clientDB?.Tables[NameTable.TableChat]?.Add(message);
            //OnSendMessage?.Invoke(this, (string)data);
        }
        //private void Events_OnDoActionWithMessage(List<Message> stationsMessages)
        //{
        //    foreach(var stationModel in SelectedStationModels)
        //    {
        //        foreach (var mesage in stationsMessages)
        //        {
        //            if(mesage.Id == stationModel.IdMaster || mesage.Id == stationModel.IdSlave)
        //            {
        //                stationModel.SelectedConnectionObject.SendTextMessage(mesage.MessageFiled);
        //            }
        //        }
        //    }

        //}

        private void UpdateSideMenu(List<TableASP> ASPList)
        {
            newWindow.UpdateSideMenu(ASPList);
        }

        private List<int> GetSideMenu()
        {
            return newWindow.SideMenuList.Select(t=>t.Id).ToList();
        }



    }
}
