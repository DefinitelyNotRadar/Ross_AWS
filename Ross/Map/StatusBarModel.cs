using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Ross.Map
{
    public class StatusBarModel : INotifyPropertyChanged
    {
        private double aJSValue = 0;
        private double rESFUSSJValue = 0;
        private double rESFUSSTDValue = 0;
        private double rESFWSJValue = 0;
        private double rESFWSTDValue = 0;


        public double AJSValue
        {
            get => aJSValue;
            set
            {
                if(aJSValue == value) return;
                aJSValue = value;
                OnPropertyChanged();
            }
        }


        public double RESFWSTDValue
        {
            get => rESFWSTDValue;
            set
            {
                if(rESFWSTDValue == value) return;
                rESFWSTDValue = value;
                OnPropertyChanged();
            }
        }


        public double RESFWSJValue
        {
            get => rESFWSJValue;
            set
            {
                if(rESFWSJValue == value) return;
                rESFWSJValue = value;
                OnPropertyChanged();
            }
        }

        public double RESFUSSTDValue
        {
            get => rESFUSSTDValue;
            set
            {
                if(rESFUSSTDValue == value) return;
                rESFUSSTDValue = value;
                OnPropertyChanged();
            }
        }

        public double RESFUSSJValue
        {
            get => rESFUSSJValue;
            set
            {
                if(rESFUSSJValue == value) return;
                rESFUSSJValue = value;
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