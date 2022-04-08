using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AdvancedWindowsAppearence.Converters
{
    internal class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string EnumString;
            try
            {
                EnumString = Enum.GetName((value.GetType()), value);
                return EnumString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
                return WallpaperSettings.WallpaperTypes.Color;

            if(value is string wallpaper)
            {
                if (wallpaper == "Image")
                    return WallpaperSettings.WallpaperTypes.Image;
                else if (wallpaper == "Slideshow")
                    return WallpaperSettings.WallpaperTypes.Slideshow;
                else if (wallpaper == "Color")
                    return WallpaperSettings.WallpaperTypes.Color;
            }

            return WallpaperSettings.WallpaperTypes.Color;
        }
    }
}
