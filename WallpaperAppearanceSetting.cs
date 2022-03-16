using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class WallpaperAppearanceSetting: AppearanceSetting
    {
        public string Path { get; private set; }
        public BoolRegistrySetting WallpaperStyleRegistrySetting { get; private set; }

        public WallpaperAppearanceSetting()
        {
            Path = GetWallpaperPath();
            WallpaperStyleRegistrySetting = new BoolRegistrySetting("Wallpaper Style", @"Control Panel\Desktop", "WallpaperStyle");
        }

        string GetWallpaperPath()
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

        public void SetWallpaper(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Wallpaper \"" + path + "\" not found!");
            Path = path;
            NotifyPropertyChanged(nameof(Path));
        }
    }
}
