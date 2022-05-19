using Ross.AddPanel;
using Ross.JSON;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ross
{
    public class MainWindowViewSize : INotifyPropertyChanged
    {
        private MarkSizeWnd markSizeWnd;

        public SizeValue sizeChat { get; set; }
        public SizeValue sizeSetting { get; set; }
        public SizeValue sizeTopTable { get; set; }
        public SizeValue sizeDownTable { get; set; }

        public SizeValue sizeLeftDownTable { get; set; }

        public MainWindowViewSize()
        {
            InitMarkSizeWnd();
        }

        private void InitMarkSizeWnd()
        {
            markSizeWnd = SerializerJSON.Deserialize<MarkSizeWnd>(AppDomain.CurrentDomain.BaseDirectory + "SizePanel.json");

            if (markSizeWnd == null)
            {
                markSizeWnd = new MarkSizeWnd();

                sizeChat = new SizeValue(DefaultSize.sizeChat);
                sizeSetting = new SizeValue(DefaultSize.sizeSetting);
                sizeTopTable = new SizeValue(DefaultSize.sizeTopTable);
                sizeDownTable = new SizeValue(DefaultSize.sizeDownTable);
                sizeLeftDownTable = new SizeValue(DefaultSize.sizeLeftDownTable);

                SetDefaultDPanel();
            }

            else
            {
                sizeChat = markSizeWnd.sizeChat;
                sizeSetting = markSizeWnd.sizeSetting;
                sizeTopTable = markSizeWnd.sizeTopTable;
                sizeDownTable = markSizeWnd.sizeDownTable;
                sizeLeftDownTable = markSizeWnd.sizeLeftDownTable;
            }

            sizeChat.PropertyChanged += Size_PropertyChanged;
            sizeSetting.PropertyChanged += Size_PropertyChanged;
            sizeTopTable.PropertyChanged += Size_PropertyChanged;
            sizeDownTable.PropertyChanged += Size_PropertyChanged;
            sizeLeftDownTable.PropertyChanged += Size_PropertyChanged;
        }

        public void SetDefaultDPanel()
        {
            sizeChat.Visible = false;
            sizeSetting.Visible = false;
            sizeTopTable.Visible = true;
            sizeDownTable.Visible = true;
            sizeLeftDownTable.Visible = true;

            sizeChat.Current = DefaultSize.sizeChat;
            sizeSetting.Current = DefaultSize.sizeSetting;
            sizeTopTable.Current = DefaultSize.sizeTopTable;
            sizeDownTable.Current = DefaultSize.sizeDownTable;
            sizeLeftDownTable.Current = DefaultSize.sizeLeftDownTable;

            sizeChat.SetDefault();
            sizeSetting.SetDefault();
            sizeTopTable.SetDefault();
            sizeDownTable.SetDefault();
            sizeLeftDownTable.SetDefault();
        }

        private void Size_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {            
            markSizeWnd.sizeChat = sizeChat;
            markSizeWnd.sizeSetting = sizeSetting;
            markSizeWnd.sizeTopTable = sizeTopTable;
            markSizeWnd.sizeLeftDownTable = sizeLeftDownTable;
            markSizeWnd.sizeDownTable = sizeDownTable;

            SerializerJSON.Serialize<MarkSizeWnd>(markSizeWnd, AppDomain.CurrentDomain.BaseDirectory + "SizePanel.json");
        }


        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        #endregion

    }
}
