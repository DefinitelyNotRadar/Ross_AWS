using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using UserControl_Chat;

namespace Ross
{
    using System.Linq;
    using System.Threading.Tasks;

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

        private async void NewWindow_OnReturnApprovedMessages(object sender, List<Message> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    if(!GetSideMenu().Contains(message.Id))
                        continue;

                    await Cliant_SendMessage(message.MessageFiled, message.Id); //TODO: check

                    var server = SelectedStationModels.FirstOrDefault(t => t.IdMaster == message.Id);
                    if (server != null && server.SelectedConnectionObject != null)
                    {
                        var result = await server.SelectedConnectionObject.SendTextMessage(message.MessageFiled).ConfigureAwait(false);

                        if (result)
                        {
                            await Client_ConfirmLastMessage(message.Id).ConfigureAwait(false);
                        }
                    }
                }
                //var lastMessage = messages.Last();
                //////TODO: not 255
                ////if (messages.Last().Id == 255)
                ////{
                ////Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                ////    {
                //        Cliant_SendMessage(lastMessage.MessageFiled, lastMessage.Id); //TODO: check
                //        //  return;
                //        //}
                //        var result = SelectedStationModels.FirstOrDefault(t => t.IdMaster == lastMessage.Id)?.SelectedConnectionObject?.SendTextMessage(lastMessage.MessageFiled);

                //        if (result == true)
                //        {
                //            Client_ConfirmLastMessage(lastMessage.Id);
                //        }
                //    //});
                
                ////if (mesage.Id == stationModel.IdMaster || mesage.Id == stationModel.IdSlave)
                ////{
                ////    stationModel.SelectedConnectionObject.SendTextMessage(mesage.MessageFiled);
                ////}         
            }
            catch { }
        }



        public async Task Cliant_SendMessage(object data, int receiver)
        {
            if (this.clientDB == null)
                return;

            //TODO: исправить 255 на универсальное
            var message = new TableChatMessage() { SenderAddress = clientAddress, ReceiverAddress = receiver, Time = DateTime.Now, Status = ChatMessageStatus.Sent, Text = data as string };
            await clientDB.Tables[NameTable.TableChat].AddAsync(message).ConfigureAwait(false);
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
