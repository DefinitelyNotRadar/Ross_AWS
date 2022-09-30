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
        [Description("Radio modem")]
        Robustel_3G_4G = 0,
        [Description("3G/4G")]
        Viper_Radio = 1,
        [Description("Ethernet")]
        Ethernet = 2,
    }

    public enum Stations : int
    {
        StationsPair1 = 0,
        StationsPair2 = 1,
        SinglStation1 = 2,
        SinglStation2 = 3,
    }
}
