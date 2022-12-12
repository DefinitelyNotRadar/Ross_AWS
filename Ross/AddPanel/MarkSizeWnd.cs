using System;
using System.ComponentModel;

namespace Ross.AddPanel
{
    public class MarkSizeWnd
    {
        public MarkSizeWnd()
        {
            sizeChat = new SizeValue(0);
            sizeSetting = new SizeValue(0);
            sizeTopTable = new SizeValue();
            sizeLeftDownTable = new SizeValue();
            sizeDownTable = new SizeValue();
            sizeDownTable.PropertyChanged += DT_PropertyChanged;
            sizeTopTable.PropertyChanged += TT_PropertyChanged;


            sizeChat.Visible = false;
            sizeSetting.Visible = false;
        }

        private void TT_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(sizeTT.Visible))
            {
                if (sizeDownTable.Visible == true && sizeTopTable.Visible == false)
                {
                    sizeDownTable.Current = -1;
                    sizeTopTable.Current = 0;
                }
            }
        }

        private void DT_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(sizeDT.Visible))
            {
                if(sizeDownTable.Visible == false && sizeTopTable.Visible == true)
                {
                    sizeDownTable.Current = 0;
                    sizeTopTable.Current = -1;
                }
            }
        }

        private SizeValue sizeC = new SizeValue(0);
        private SizeValue sizeS = new SizeValue(0);
        private SizeValue sizeTT = new SizeValue();
        private SizeValue sizeLDT = new SizeValue();
        private SizeValue sizeDT = new SizeValue();




        public SizeValue sizeChat 
        {
            get => sizeC;
            set
            {
                sizeC = value;
            }
        } 
        public SizeValue sizeSetting 
        {
            get => sizeS;
            set
            {
                sizeS = value;
            }
        }
        public SizeValue sizeTopTable 
        {
            get => sizeTT;
            set
            {
                sizeTT = value;
            }
        }
        public SizeValue sizeLeftDownTable 
        {
            get => sizeLDT;
            set
            {
                sizeLDT = value;
            }
        }
        public SizeValue sizeDownTable 
        {
            get => sizeDT;
            set
            {                
                sizeDT = value;
            }
        }
    }
}