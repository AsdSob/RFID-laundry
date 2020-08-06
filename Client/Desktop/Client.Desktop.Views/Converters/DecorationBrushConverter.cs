using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Client.Desktop.ViewModels.Common.Services;

namespace Client.Desktop.Views.Converters
{
    public class DecorationBrushConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var decorationType = (value as IDecorationItem)?.ItemDecorationType;
            if (decorationType == null) return null;

            if (decorationType == ItemDecorationType.Registered) 
                return Brushes.LightGreen;
            
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
