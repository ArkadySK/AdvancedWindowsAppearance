using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        public WallpaperAppearanceSetting Wallpaper { get; } = new WallpaperAppearanceSetting();

        public SlideshowSettings Slideshow { get; } = new SlideshowSettings();

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateWallpaperStyle()
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
                        break;
                    }
                case WallpaperTypes.Slideshow:
                    {
                        Wallpaper.SetWallpaper(Environment.SpecialFolder.ApplicationData.ToString() + @"\Microsoft\Windows\Themes\TranscodedWallpaper");
                        Wallpaper.SaveToRegistry();
                        throw new Exception("You cannot save slideshow into registry, use save as theme instead");
                    }
                case WallpaperTypes.Color:
                    {
                        Wallpaper.SetWallpaper("");
                        Wallpaper.SaveToRegistry();
                        break;
                    }
            }
            if(BackgroundColor.IsEdited)
                BackgroundColor.SaveToRegistry();

        }

    }
}
