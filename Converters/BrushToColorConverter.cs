using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AdvancedWindowsAppearence.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert(object brush, Type targetType, object parameter, CultureInfo culture)
        {
            if(brush is System.Drawing.Color color)
            {
                return MediaColorToBrush(color);
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static Brush MediaColorToBrush(System.Drawing.Color? col)
        {
            if (col == null)
                return null;

            Brush returnbrush = new SolidColorBrush(Color.FromArgb(col.Value.A, col.Value.R, col.Value.G, col.Value.B));
            return returnbrush;
        }
    }    
}
