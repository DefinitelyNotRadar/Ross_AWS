using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ross.Converters
{
    public class DoubleGridLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double))
                return Binding.DoNothing;

            if ((double)value == -1)
                return new GridLength(1, GridUnitType.Star);

            return new GridLength((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var gridLength = (GridLength)value;
            return gridLength.Value;
        }
    }
}