using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Ross.Converters
{
    public class BooleanToGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gridLength = (GridLength)value;
            if (gridLength.Value == 0)
                return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isChacked = (bool)value;
            GridLength gridLength;
            if (isChacked)
                gridLength = new GridLength(300, GridUnitType.Pixel);
            else
                gridLength = new GridLength(0, GridUnitType.Pixel);

            return gridLength;
        }
    }
}
