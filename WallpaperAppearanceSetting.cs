using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class WallpaperAppearanceSetting: AppearanceSetting
    {
        public string WallpaperPath;
        public WallpaperAppearanceSetting(string _name, string _wallpaperPath)
        {
            Name = _name;
            WallpaperPath = _wallpaperPath;
            if (_wallpaperPath == "")
            {
                WallpaperPath = GetWallpaperPath();
            }
        }

        public string GetWallpaperPath()
        {
            try
            {
                return Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop").GetValue("Wallpaper").ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
