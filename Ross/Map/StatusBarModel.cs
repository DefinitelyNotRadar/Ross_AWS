using System.Windows;

namespace Ross.Map
{
    public partial class MapLayout : Window
    {
        private double aJSValue;
        private double rESFUSSJValue;
        private double rESFUSSTDValue;
        private double rESFWSJValue;
        private double rESFWSTDValue;


        public double AJSValue
        {
            get => aJSValue;
            set
            {
                aJSValue = value;
                AJS.Text = value.ToString();
            }
        }


        public double RESFWSTDValue
        {
            get => rESFWSTDValue;
            set
            {
                rESFWSTDValue = value;
                RESFWSTD.Text = value.ToString();
            }
        }


        public double RESFWSJValue
        {
            get => rESFWSJValue;
            set
            {
                rESFWSJValue = value;
                RESFWSJ.Text = value.ToString();
            }
        }

        public double RESFUSSTDValue
        {
            get => rESFUSSTDValue;
            set
            {
                rESFUSSTDValue = value;
                RESFUSSTD.Text = value.ToString();
            }
        }

        public double RESFUSSJValue
        {
            get => rESFUSSJValue;
            set
            {
                rESFUSSJValue = value;
                RESFUSSJ.Text = value.ToString();
            }
        }
    }
}