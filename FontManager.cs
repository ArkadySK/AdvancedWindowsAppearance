using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AdvancedWindowsAppearence
{
    static class FontManager
    {
        public static double DPI = -1.0;
        static bool _loaded = false;
        static bool _loading = true;
        static List<Font> _fonts = new List<Font>();

        public static void GetDPI()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            if (key == null)
            {
                DPI = 1d;
                return;
            }
            DPI = (int)key.GetValue("AppliedDPI") / 96d;
        }

        public static List<Font> GetSystemFonts()
        {
            if (!_loaded)
            {
                _loaded = true;
                foreach (FontFamily fontfamily in FontFamily.Families)
                {
                    try
                    {
                        Font f = new Font(fontfamily, 9);
                        _fonts.Add(f);
                    }
                    catch { }

                }
                _loading = false;
            }
            while (_loading) { }
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
