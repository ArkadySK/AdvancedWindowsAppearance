using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AdvancedWindowsAppearence.Converters
{
    internal class BooleanToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isBold = (bool)value;
            if (isBold)
                return FontWeights.Bold;
            else
                return FontWeights.Regular;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
