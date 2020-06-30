using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.Desktop.Views.Converters
{
    public class DataGridRowColorConvertor: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Transparent;
            var str = value is bool;

            if (str) return Brushes.LightGreen;
            
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
