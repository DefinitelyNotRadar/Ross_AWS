using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ross.Converters
{
    public class BoolToRowHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isChacked = (bool)value;
            GridLength gridLength;
            if (isChacked)
                gridLength = new GridLength(1, GridUnitType.Star);
            else
                gridLength = new GridLength(0, GridUnitType.Star);

            return gridLength;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}