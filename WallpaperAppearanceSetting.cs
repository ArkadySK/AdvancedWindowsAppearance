using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class WallpaperAppearanceSetting : AppearanceSetting
    {
        public string Path { get; private set; }
        public WallpaperStyleRegistrySetting WallpaperStyleRegistrySetting { get; private set; }

        public WallpaperAppearanceSetting()
        {
            Path = GetWallpaperPath();
            WallpaperStyleRegistrySetting = new WallpaperStyleRegistrySetting("WallpaperStyle");
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

    public class WallpaperStyleRegistrySetting: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public string[] WallpaperStyles { get; private set; }
        public enum WallpaperStyle
        {
            Center = 0,
            Tile = 1,
            Stretched = 2,
            Fit = 3,
            Fill = 4,
            Span = 5
        }

        public WallpaperStyle SelectedWallpaperStyle
        { 
            get => _style; 
            set
            {
                _style = value;
                NotifyPropertyChanged();
            }
        }

        private WallpaperStyle _style;

        

        internal WallpaperStyleRegistrySetting(string registryKey)
        {
            WallpaperStyles = new string[] { "Center", "Tile", "Stretched", "Fit", "Fill", "Span" };

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
                int value = int.Parse(regKey.GetValue(registryKey, 0).ToString());
                SelectedWallpaperStyle = (WallpaperStyle)value;
            }
            catch (Exception ex)
            {

            }
        }

        internal void SaveToRegistry()
        {
            BoolRegistrySetting tileWallpaperRegistrySetting = new BoolRegistrySetting("Tile Wallpaper", @"Control Panel\Desktop", "TileWallpaper");
            if (SelectedWallpaperStyle == WallpaperStyle.Tile)
                tileWallpaperRegistrySetting.Checked = true;
            else
                tileWallpaperRegistrySetting.Checked = false;
            tileWallpaperRegistrySetting.SaveToRegistry();
        }
    }
}
