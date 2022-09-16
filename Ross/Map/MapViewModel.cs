using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DLLSettingsControlPointForMap.Model;

namespace Ross.Map
{
    public class MapViewModel : INotifyPropertyChanged
    {

        private StatusBarModel statusBar = new StatusBarModel();
        private GridLength settingsControlWidth = new GridLength(0, GridUnitType.Pixel);


        public StatusBarModel StatusBar
        {
            get => statusBar;
            set
            {
                if (statusBar == value) return;
                statusBar = value;
                OnPropertyChanged();
            }
        }

        public GridLength SettingsControlWidth
        {
            get => settingsControlWidth;
            set
            {
                if(settingsControlWidth == value) return;
                settingsControlWidth = value;
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
