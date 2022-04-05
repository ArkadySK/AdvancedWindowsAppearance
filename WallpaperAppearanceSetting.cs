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
        public WallpaperStyleRegistrySetting WallpaperStyleSettings { get; private set; }

        public WallpaperAppearanceSetting()
        {
            Path = GetWallpaperPath();
            WallpaperStyleSettings = new WallpaperStyleRegistrySetting("WallpaperStyle");
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

        public void SaveToRegistry()
        {
            //save the wallpaper
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            registryKey.SetValue("Wallpaper", Path, RegistryValueKind.String);
            registryKey.Close();
            //save the wallpaper styles
            WallpaperStyleSettings.SaveToRegistry();
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
            Fit = 6,
            Fill = 10,
            Span = 22
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
            BoolRegistrySetting tileWallpaperRegistrySetting = new BoolRegistrySetting("Tile Wallpaper", @"Control Panel\Desktop", "TileWallpaper");
            WallpaperStyles = new string[] { "Center", "Tile", "Stretched", "Fit", "Fill", "Span" };

            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false);
                int value = int.Parse(regKey.GetValue(registryKey, 0).ToString());
                regKey.Close();
                if(tileWallpaperRegistrySetting.Checked.Value == true)
                    value = 1;

                SelectedWallpaperStyle = (WallpaperStyle)value;
            }
            catch (Exception ex)
            {

            }
        }

        internal void SaveToRegistry()
        {
            //save the tile
            BoolRegistrySetting tileWallpaperRegistrySetting = new BoolRegistrySetting("Tile Wallpaper", @"Control Panel\Desktop", "TileWallpaper");
            if (SelectedWallpaperStyle == WallpaperStyle.Tile)
                tileWallpaperRegistrySetting.Checked = true;
            else
                tileWallpaperRegistrySetting.Checked = false;
            tileWallpaperRegistrySetting.SaveToRegistry();

            //save the other styles
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            regKey.SetValue("WallpaperStyle", (int)SelectedWallpaperStyle, RegistryValueKind.String);
            regKey.Close();
        }
    }
}
