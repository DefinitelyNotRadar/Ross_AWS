using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserControl_Chat;

namespace Ross
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public Chat()
        {
            InitializeComponent();
        }

        List<StationClassForChat> sideMenuList;

        public void SetStations()
        {
            List<StationClassForChat> testList = new List<StationClassForChat>()
            {
                new StationClassForChat(0,"CP", true)
            };
            curChat.InitStations(testList);

            // += DrawMessageToChat;
            Events.OnGetStationsMessage += DrawMessageToChat;
            Events.OnSendStationsMessage += ReturnApprovedMessages;
        }

        public void UpdateSideMenu(List<TableASP> ASPList)
        {
            try
            {
                sideMenuList = new List<StationClassForChat>();
                ASPList.ForEach(ASPMember =>
                {
                    if(!ASPMember.ISOwn)
                    sideMenuList.Add(new StationClassForChat(ASPMember.Id, ASPMember.CallSign, true));
                });
                //sideMenuList.Add(new StationClassForChat(0, "ПУ", true));
                curChat.UpdateSideMenuMembers(sideMenuList);
            }
            catch (Exception)
            { }
        }


        private void ReturnApprovedMessages(List<Message> stationsMessages)
        {
            foreach (Message curStationsMessage in stationsMessages)
            {
                curStationsMessage.IsTransmited = true;
                curStationsMessage.SenderName = curStationsMessage.SenderName;
                curStationsMessage.MessageFontSize = 20;
                curStationsMessage.SenderNameFontSize = 15;

                //curStationsMessage.IsTransmited = false;
            }
            curChat.DrawMessageToChat(stationsMessages);
        }

        public void DrawMessageToChat(List<Message> stationsMessages)
        {
            curChat.DrawMessageToChat(stationsMessages);
        }
    }
}
