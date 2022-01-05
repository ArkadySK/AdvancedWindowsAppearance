using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace AdvancedWindowsAppearence.Converters
{
    public class MultiMarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            /*double x = double.Parse(values[0].ToString()) / 5d;
            double y = double.Parse(values[1].ToString()) / 5d;
            return new Thickness(x, y, x, y);*/
            return null;
            
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
