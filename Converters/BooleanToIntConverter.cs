using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdvancedWindowsAppearence.Converters
{
    public class BooleanToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) 
                return 0;

            if(value is bool boolValue)
                return boolValue ? 1 : 0;
            return 0;
                
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return false;
            if (value is int intValue)
                if (intValue == 0)
                    return false;
            return true;
        }
    }
}
