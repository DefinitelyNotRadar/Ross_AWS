using System;
using System.Globalization;
using System.Windows.Data;
using DLLSettingsControlPointForMap.Model;

namespace Ross.Converters
{
    public class ViewCoordsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "DD.dddddd":
                    return 1;

                case "DD MM.mmmm":
                    return 2;

                case "DD MM SS.ss":
                    return 3;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    internal class CoordViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((CoordView)value == CoordView.Dd)
                return "DD";
            if ((CoordView)value == CoordView.DMm)
                return "DD_MM_mm";
            if ((CoordView)value == CoordView.DMSs)
                return "DD_MM_SS_ss";
            return "DD_dd";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    
}