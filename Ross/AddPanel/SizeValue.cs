using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ross.AddPanel
{
    public class SizeValue : INotifyPropertyChanged
    {
        private double _initial = 0;
        private double _default;

        private bool _visible;

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    OnPropertyChanged();
                    UpdateValue();
                }
            }
        }

        private double _current;

        public double Current
        {
            get { return _current; }
            set
            {
                if (_current != value)
                {
                    _last = _current;
                    _current = value;
                    OnPropertyChanged();

                }
            }
        }

        private double _last;

        public double Last
        {
            get { return _last; }
            set
            {
                if (_last != value)
                {
                    _last = value;
                }
            }
        }

        public SizeValue(double Deafult)
        {
            _default = Deafult;
            _last = Deafult;
            Visible = true;

        }

        public SizeValue()
        {
            _default = 100;
            _last = 100;
            Visible = true;

        }

        public void SetDefault()
        {

        }

        private void UpdateValue()
        {
            Current = (bool)Visible ? _last : _initial;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));

        }
    }
}
