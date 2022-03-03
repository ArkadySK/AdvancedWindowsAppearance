using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Effects;

namespace AdvancedWindowsAppearence.Converters
{
    internal class BooleanToBlurEffectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool enabled = (bool)value;

            if (!enabled)
                return null;
            BlurEffect blurEffect = new BlurEffect();
            blurEffect.Radius = 5;
            return blurEffect;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
