using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    static class FontManager
    {
        public static double DPI = 1.0f;
        static bool _loaded = false;
        static List<Font> _fonts = new List<Font>();

        public static void GetDPI(float samplefontsize)
        {
            FontManager.DPI = System.Windows.SystemFonts.CaptionFontSize / samplefontsize;
        }

        public static List<Font> GetSystemFonts()
        {
            if (!_loaded)
            {
                foreach (FontFamily fontfamily in FontFamily.Families)
                {
                    try
                    {
                        Font f = new Font(fontfamily, 9);
                        _fonts.Add(f);
                    }
                    catch { }

                }
                _loaded = true;
            }
            return _fonts;
        }

        public static Font FindFontFromString(string stringname)
        {
            List<Font> fonts = GetSystemFonts();
            foreach (var f in fonts)
            {
                if (stringname.Contains(f.Name))
                {
                    return f;
                }
            }
            Console.WriteLine("Font not found!");
            return null;
        }
    }
}
