using DLLSettingsControlPointForMap.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ross.Converters
{
    internal class CoordViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((CoordView)value == CoordView.Dd)
                return "DD";
            else if ((CoordView)value == CoordView.DMm)
                return "DD_MM_mm";
            else if ((CoordView)value == CoordView.DMSs)
                return "DD_MM_SS_ss";
            else return "DD_dd";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
