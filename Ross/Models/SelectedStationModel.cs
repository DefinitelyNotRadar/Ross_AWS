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
        private ConnectionTypeServerOD connectionType = ConnectionTypeServerOD.Viper_Radio;
        private int id_master;
        private int id_slave;
        private string ipAddress = " ";
        private int port = 0;
        private string ipAddress_3G4G = " ";
        private int port_3G4G = 0;

        private readonly object locker = new object();


        public GrpcClient SelectedConnectionObject
        {
            get { lock (locker) { return selectedConnectionObject; } }
            set
            {
                lock (locker)
                {
                    if (selectedConnectionObject != null)
                        if (selectedConnectionObject.Equals(value)) return;
                    selectedConnectionObject = value;
                    OnPropertyChanged();
                }
            }
        }

        public ConnectionTypeServerOD ConnectionTypeServerOD
        {
            get { lock (locker) { return connectionType; } }
            set
            {
                if (connectionType == value) return;
                connectionType = value;
                OnPropertyChanged();
            }
        }

        public int IdMaster
        {
            get {lock (locker) { return id_master; } }
            set
            {
                if(id_master == value) return;
                id_master = value;
                OnPropertyChanged();
            }
        }

        public int IdSlave
        {
            get { lock (locker) { return id_slave; } }
            set
            {
                if (id_slave == value) return;
                id_slave = value;
                OnPropertyChanged();
            }
        }

        public string IpAddress_interior
        {
            get { lock (locker) { return ipAddress; } }
            set
            {
                if(ipAddress == value) return;
                ipAddress = value;
                OnPropertyChanged();
            }
        }

        public int Port_interior
        {
            get { lock (locker) { return port; } }
            set
            {
                if(port == value) return;
                port = value;
                OnPropertyChanged();
            }
        }


        public string IpAddress_interior_3G4G
        {
            get { lock (locker) { return ipAddress_3G4G; } }
            set
            {
                if (ipAddress_3G4G == value) return;
                ipAddress_3G4G = value;
                OnPropertyChanged();
            }
        }

        public int Port_interior_3G4G
        {
            get { lock (locker) { return port_3G4G; } }
            set
            {
                if (port_3G4G == value) return;
                port_3G4G = value;
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
