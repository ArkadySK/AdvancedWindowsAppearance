using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AdvancedWindowsAppearence
{
    static class FontManager
    {
        public static double DPI = -1.0f;
        static bool _loaded = false;
        static List<Font> _fonts = new List<Font>();

        public static void GetDPI()
        {
            /*if (samplefontsize == 0)
                return;
            DPI = System.Windows.SystemFonts.CaptionFontSize / samplefontsize;
            */
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            if (key == null)
            {
                DPI = 1;
                return;
            }
            Console.WriteLine();
            DPI = (int)key.GetValue("AppliedDPI") / 96f;
            Console.WriteLine(SystemFonts.MenuFont.Size);

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
                if (f is null) continue;
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
