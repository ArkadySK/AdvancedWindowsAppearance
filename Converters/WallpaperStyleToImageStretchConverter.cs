using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using static AdvancedWindowsAppearence.WallpaperStyleRegistrySetting;

namespace AdvancedWindowsAppearence.Converters
{
    internal class WallpaperStyleToImageStretchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is WallpaperStyle wallpaperStyle)
            {
                switch (wallpaperStyle)
                {
                    //ok = implemented
                    case WallpaperStyle.Center: 
                        return System.Windows.Media.Stretch.Uniform;
                    case WallpaperStyle.Fill: //ok
                        return System.Windows.Media.Stretch.UniformToFill;
                    case WallpaperStyle.Tile:
                        return System.Windows.Media.Stretch.None;
                    case WallpaperStyle.Span: //ok
                        return System.Windows.Media.Stretch.UniformToFill;
                    case WallpaperStyle.Fit: //ok
                        return System.Windows.Media.Stretch.Uniform;
                    case WallpaperStyle.Stretched: //ok
                        return System.Windows.Media.Stretch.Fill;
                }

            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
