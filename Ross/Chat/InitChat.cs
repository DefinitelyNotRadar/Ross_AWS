using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using UserControl_Chat;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private Chat newWindow;
        private Buble chatBuble;

        private void InitChat()
        {
            newWindow = new Chat();
            chatBuble = new Buble();
            newWindow.OnReturnApprovedMessages += NewWindow_OnReturnApprovedMessages;
            newWindow.SetStations();
          
        }

        private void NewWindow_OnReturnApprovedMessages(object sender, List<Message> messages)
        {
            try
            {
                if (messages.Last().Id == 0)
                {
                    ClientBErezina_SendTextMessage(messages.Last().MessageFiled);
                    return;
                }
                SendMessage(messages.Last().Id, messages.Last().MessageFiled);                
            }
            catch { }
        }

        private void UpdateSideMenu(List<TableJammerStation> ASPList)
        {
            newWindow.UpdateSideMenu(ASPList);
        }
    }
}
