using ClientDataBase.ServiceDB;
using ModelsTablesDBLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventArgs = System.EventArgs;

namespace Ross.Map._EventArgs
{
    public class CoordEventArgs : EventArgs
    {
        public Coord Data
        {
            get;
            private set;
        }

        public CoordEventArgs(Coord data)
        {
            Data = data;
        }

    }
}
