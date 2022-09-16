using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UserControl_Chat;

namespace Ross
{
    public partial class MainWindow : Window
    {
        private void DrawMessageToChat(Message curMessage)
        {
            List<Message> curMessages = new List<Message>();

            curMessage.IsTransmited = true;
            curMessage.IsSendByMe = Roles.Received;
            curMessages.Add(curMessage);
          
            Dispatcher.Invoke(() =>
            {
                UserControlChat.DrawMessageToChat(curMessages);
            });

        }
    }
}
