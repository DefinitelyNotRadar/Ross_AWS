using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ross.Models
{
    public enum ConnectionTypeServerOD
    {
        [Description("Ethernet")]
        Ethernet = 0,
        [Description("Viper")]
        Robustel_3G_4G = 1,
        [Description("3G/4G")]
        Viper_Radio = 2,
    }
}
