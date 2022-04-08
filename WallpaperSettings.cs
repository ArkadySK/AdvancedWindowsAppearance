using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class WallpaperSettings
    {

        public WallpaperAppearanceSetting Wallpaper { get;} = new WallpaperAppearanceSetting();

        public enum WallpaperTypes
        {
            Image,
            Slideshow,
            Color
        }
        public WallpaperTypes WallpaperType;


        public void UpdateWallpaperStyle()
        {
            //if it is solid color
            if (String.IsNullOrWhiteSpace(Wallpaper.Path))
            {
                WallpaperType = WallpaperTypes.Color;
                return;
            }

            //if it is a slideshow
            if (Wallpaper.Path.Contains("Transcoded"))
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
                        throw new NotImplementedException();
                    }
            }

        }

    }
}
