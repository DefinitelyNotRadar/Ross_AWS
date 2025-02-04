﻿using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UserControl_Chat;

namespace Ross
{
    using System.Threading;

    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public Chat()
        {
            InitializeComponent();
        }

        public event EventHandler<List<Message>> OnReturnApprovedMessages;
        public List<StationClassForChat> SideMenuList;

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
            //Events.OnClosingChat += HideWindow;
        }

        public void UpdateSideMenu(List<TableASP> ASPList)
        {
            try
            {
                SideMenuList = new List<StationClassForChat>();
                ASPList.ForEach(ASPMember =>
                {
                    if(!ASPMember.ISOwn && ASPMember.Role != RoleStation.Slave)
                        SideMenuList.Add(new StationClassForChat(ASPMember.Id, ASPMember.CallSign, true));
                });
                //sideMenuList.Add(new StationClassForChat(0, "ПУ", true));
                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    curChat.UpdateSideMenuMembers(SideMenuList);
                });
                
            }
            catch (Exception)
            { }
        }


        private void ReturnApprovedMessages(List<Message> stationsMessages)
        {
            var foWork = new List<Message>();
            foreach (Message curStationsMessage in stationsMessages)
            {
                if(SideMenuList.FirstOrDefault(t=>t.Id == curStationsMessage.Id)== null)
                    continue;
                //curStationsMessage.IsTransmited = true;
                curStationsMessage.SenderName = curStationsMessage.SenderName;
                curStationsMessage.MessageFontSize = 20;
                curStationsMessage.SenderNameFontSize = 15;
                foWork.Add(curStationsMessage);
                //curStationsMessage.IsTransmited = false;
            }
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                curChat.DrawMessageToChat(foWork);
            });

            OnReturnApprovedMessages?.Invoke(this, foWork);
        }

        public void DrawReceivedMessage(int address, string message)
        {
            List<Message> curMessages = new List<Message>();
            curMessages.Add(new Message
                                {
                                    MessageFiled = message,
                                    Id = address,
                                    IsTransmited = true,
                                    IsSendByMe = Roles.Received

                                });

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    curChat.DrawMessageToChat(curMessages);
                });

        }

        public void DrawMessageToChat(List<Message> stationsMessages)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                curChat.DrawMessageToChat(stationsMessages);
            });
        }

        //private void HideWindow()
        //{
        //    this.Visibility = Visibility.Hidden;
        //}

        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    base.OnMouseLeftButtonDown(e);

        //    // Begin dragging the window
        //    this.DragMove();
        //}

        public void SetLanguage(Languages language)
        {
            ResourceDictionary dict = new ResourceDictionary();
            try
            {
                switch (language)
                {
                    case Languages.Eng:
                        dict.Source = new Uri("/Ross;component/Languages/UIChat/StringResource.EN.xaml",
                            UriKind.Relative);
                        break;

                    case Languages.Rus:
                        dict.Source = new Uri("/Ross;component/Languages/UIChat/StringResource.RU.xaml",
                            UriKind.Relative);
                        break;


                    default:
                        dict.Source = new Uri("/Ross;component/Languages/UIChat/StringResource.EN.xaml",
                            UriKind.Relative);
                        break;
                }

                this.Resources.MergedDictionaries.Add(dict);
            }
            catch (Exception ex)
            { }
        }

        public void ConfirmSentMessage(int NumJammer)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (ThreadStart)delegate ()
                {
                    curChat.ConfirmMessage(new Message() { Id = NumJammer });
                });
        }

    }
}
