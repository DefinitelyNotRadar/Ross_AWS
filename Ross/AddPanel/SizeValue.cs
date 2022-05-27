using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ross.AddPanel
{
    public class SizeValue : INotifyPropertyChanged
    {
        private readonly double _initial = 0;
        private double _current;
        private double _default;

        private double _last;

        private bool _visible;

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

        public bool Visible
        {
            get => _visible;
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

        public double Current
        {
            get => _current;
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

        public double Last
        {
            get => _last;
            set
            {
                if (_last != value) _last = value;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void SetDefault()
        {
        }

        private void UpdateValue()
        {
            Current = Visible ? _last : _initial;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}