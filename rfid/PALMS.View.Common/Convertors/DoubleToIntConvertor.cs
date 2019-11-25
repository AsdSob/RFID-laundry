using System;
using System.Globalization;
using System.Windows.Data;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.View.Common.Converters
{
    public class DoubleToIntConvertor: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var priceUnit = (int)value;

            if(priceUnit == (int)LinenUnitEnum.Piece)
                return "n0";

            return "n2";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
