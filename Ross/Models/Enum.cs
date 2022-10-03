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
        Viper_Radio = 0,
        [Description("3G/4G")]
        Robustel_3G_4G = 1,
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

    public enum Liters
    {
        L13,
        L24,
        L59,
        L10
    }
}
