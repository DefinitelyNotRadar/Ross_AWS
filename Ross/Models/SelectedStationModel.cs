using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TransmissionLib.GrpcTransmission;

namespace Ross.Models
{
    public class SelectedStationModel: INotifyPropertyChanged
    {
        private GrpcClient selectedConnectionObject;
        private int id_master;
        private int id_slave;
        private string ipAddress;
        private int port;

        public GrpcClient SelectedConnectionObject
        {
            get => selectedConnectionObject;
            set
            {
                if(selectedConnectionObject != null)
                    if (selectedConnectionObject.Equals(value)) return;
                selectedConnectionObject = value;
                OnPropertyChanged();
            }
        }

        public int IdMaster
        {
            get => id_master;
            set
            {
                if(id_master == value) return;
                id_master = value;
                OnPropertyChanged();
            }
        }

        public int IdSlave
        {
            get => id_slave;
            set
            {
                if (id_slave == value) return;
                id_slave = value;
                OnPropertyChanged();
            }
        }

        public string IpAddress_interior
        {
            get => ipAddress;
            set
            {
                if(ipAddress == value) return;
                ipAddress = value;
                OnPropertyChanged();
            }
        }

        public int Port_interior
        {
            get => port;
            set
            {
                if(port == value) return;
                port = value;
                OnPropertyChanged();
            }
        }

        #region NotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            try
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
            catch { }
        }

        #endregion
    }
}
