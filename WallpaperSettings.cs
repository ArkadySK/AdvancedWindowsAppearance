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
    public class WallpaperSettings : INotifyPropertyChanged
    {

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public WallpaperAppearanceSetting Wallpaper { get; private set; }

        public SlideshowSettings Slideshow { get; private set; }

        public ColorAppearanceSetting BackgroundColor { get; private set; }

        public WallpaperTypes WallpaperType
        {
            get => _wallpaperType;
            set
            {
                _wallpaperType = value;
                NotifyPropertyChanged();
            }
        }

        internal void CreateDefaultSlideshow()
        {
            Slideshow = new SlideshowSettings();
            string WindowsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            if (Directory.Exists(WindowsFolder + @"\Web\Wallpaper\Theme1"))
            {
                Slideshow.Folder = WindowsFolder + @"\Web\Wallpaper\Theme1";
            }
            else
            {
                Slideshow.Folder = WindowsFolder + @"\Web\Wallpaper\Windows";
            }
        }

        public string[] WallpaperTypesStrings { get; } = new string[] { "Image", "Slideshow", "Color" };

        public enum WallpaperTypes 
        {
            Image = 0,
            Slideshow = 1,
            Color = 2
        }
        private WallpaperTypes _wallpaperType;

        /// <summary>
        /// Constructor for the Wallpaper settings
        /// </summary>
        /// <param name="backgroundColor">color of the wallpaper's background</param>
        public WallpaperSettings(ColorAppearanceSetting backgroundColor)
        {
            BackgroundColor = backgroundColor;
            Wallpaper = new WallpaperAppearanceSetting();
            UpdateWallpaperType(); 

            if (WallpaperType == WallpaperTypes.Slideshow)
            Slideshow = new SlideshowSettings();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateWallpaperType()
        {
            //if it is solid color
            if (String.IsNullOrWhiteSpace(Wallpaper.Path))
            {
                WallpaperType = WallpaperTypes.Color;
                return;
            }

            //if it is a slideshow
            if (Wallpaper.Path.Contains("\\TranscodedWallpaper"))
            {
                WallpaperType = WallpaperTypes.Slideshow;
            }
            //if it is a single image wallpaper
            else
            {
                WallpaperType = WallpaperTypes.Image;
            }
        }

        public void SaveToRegistry()
        {
            switch (WallpaperType)
            {
                case WallpaperTypes.Image:
                    {
                        Wallpaper.SaveToRegistry();
                        Slideshow?.DeleteIni();
                        WindowsInteropServices.Win32Methods.UpdateWallpaper(Wallpaper.Path);
                        break;
                    }
                case WallpaperTypes.Slideshow:
                    {
                        string oldPath = Wallpaper.Path;
                        try
                        {
                            Wallpaper.Path = Environment.SpecialFolder.ApplicationData.ToString() + @"\Microsoft\Windows\Themes\TranscodedWallpaper";
                            Slideshow.SaveToIni();
                        }
                        catch (Exception ex)
                        {
                            Wallpaper.Path = oldPath;
                            throw ex;
                        }
                        break;
                    }
                case WallpaperTypes.Color:
                    {
                        Wallpaper.Path = "";
                        Wallpaper.SaveToRegistry();
                        Slideshow?.DeleteIni();
                        WindowsInteropServices.Win32Methods.UpdateWallpaper("");
                        break;
                    }
            }
            if(BackgroundColor.IsEdited)
                BackgroundColor.SaveToRegistry();
        }

    }
}
