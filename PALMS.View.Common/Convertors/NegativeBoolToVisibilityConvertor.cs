using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PALMS.View.Common.Converters
{
    public class NegativeBoolToVisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = value is bool ? (bool) value : false;

            return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
