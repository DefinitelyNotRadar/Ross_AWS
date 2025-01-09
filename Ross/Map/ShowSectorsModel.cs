using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ross.Map
{
    public class ShowSectorsModel : INotifyPropertyChanged
    {

        private bool isShowLPA510Sector = true;
        private bool isShowBPSSSector = true;
        private bool isShowLPA13Sector = true;
        private bool isShowLPA24Sector = true;

        public bool IsShowLPA510Sector
        {
            get => isShowLPA510Sector;
            set
            {
                if (isShowLPA510Sector == value) return;
                isShowLPA510Sector = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowLBPSSSector
        {
            get => isShowBPSSSector;
            set
            {
                if (isShowBPSSSector == value) return;
                isShowBPSSSector = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowLPA13Sector
        {
            get => isShowLPA13Sector;
            set
            {
                if (isShowLPA13Sector == value) return;
                isShowLPA13Sector = value;
                OnPropertyChanged();
            }
        }

        public bool IsShowLPA24Sector
        {
            get => isShowLPA24Sector;
            set
            {
                if (isShowLPA24Sector == value) return;
                isShowLPA24Sector = value;
                OnPropertyChanged();
            }
        }


        #region OnPropertyChanged

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
