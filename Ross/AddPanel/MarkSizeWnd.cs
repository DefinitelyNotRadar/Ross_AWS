using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ross.AddPanel
{
    public class MarkSizeWnd
    {
        public SizeValue sizeChat { get; set; } = new SizeValue(0);
        public SizeValue sizeSetting { get; set; } = new SizeValue(0);
        public SizeValue sizeTopTable { get; set; } = new SizeValue();
        public SizeValue sizeLeftDownTable { get; set; } = new SizeValue();
        public SizeValue sizeDownTable { get; set; } = new SizeValue();

        public MarkSizeWnd()
        {
            sizeChat = new SizeValue(0);
            sizeSetting = new SizeValue(0);
            sizeTopTable = new SizeValue();
            sizeLeftDownTable = new SizeValue();
            sizeDownTable = new SizeValue();

            sizeChat.Visible = false;
            sizeSetting.Visible = false;

        }
    }
}
