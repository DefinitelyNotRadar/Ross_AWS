using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ross.AddPanel
{
    public class MarkSizeWnd
    {
        public SizeValue sizeChat { get; set; } = new SizeValue();
        public SizeValue sizeSetting { get; set; } = new SizeValue();
        public SizeValue sizeTopTable { get; set; } = new SizeValue();
        public SizeValue sizeLeftDownTable { get; set; } = new SizeValue();
        public SizeValue sizeRightDownTable { get; set; } = new SizeValue();

        public MarkSizeWnd()
        {
            sizeChat = new SizeValue();
            sizeSetting = new SizeValue();
            sizeTopTable = new SizeValue();
            sizeLeftDownTable = new SizeValue();
            sizeRightDownTable = new SizeValue();

        }
    }
}
