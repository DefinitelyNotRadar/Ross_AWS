using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ross.Map
{
    public partial class MapLayout : Window
    {
        private double aJSValue = 0;
        private double rESFWSTDValue = 0;
        private double rESFWSJValue = 0;
        private double rESFUSSTDValue = 0;
        private double rESFUSSJValue = 0;


        public double AJSValue {
            get { 
                return aJSValue; 
            }
            set {
                aJSValue = value; 
                AJS.Text = value.ToString();
            }
        }


        public double RESFWSTDValue
        {
            get
            {
                return rESFWSTDValue;
            }
            set
            {
                rESFWSTDValue = value;
                RESFWSTD.Text = value.ToString();
            }
        }


        public double RESFWSJValue
        {
            get
            {
                return rESFWSJValue;
            }
            set
            {
                rESFWSJValue = value;
                RESFWSJ.Text = value.ToString();
            }
        }

        public double RESFUSSTDValue
        {
            get
            {
                return rESFUSSTDValue;
            }
            set
            {
                rESFUSSTDValue = value;
                RESFUSSTD.Text = value.ToString();
            }
        }

        public double RESFUSSJValue
        {
            get
            {
                return rESFUSSJValue;
            }
            set
            {
                rESFUSSJValue = value;
                RESFUSSJ.Text = value.ToString();
            }
        }
    }
}
