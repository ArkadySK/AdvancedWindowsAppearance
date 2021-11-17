using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    internal class FontAppearanceSetting: AppearanceSetting
    {
        public Font Font;
        public bool Font_isBold;
        public bool Font_isItalic;
        public int FontSize;
        readonly string FontRegistryPath;
        public readonly string FontColorRegistryPath;
        public Color? FontColor;
        public string FontColorValue;
        readonly int fontNameStartIndex = 28; //odtialto zacina string (nazov fontu) vramci jedneko keyu v registri


        public FontAppearanceSetting(string _name, string _regeditPath, string _colorRegistryPath)
        {
            Name = _name;
            FontRegistryPath = _regeditPath;
            FontColorRegistryPath = _colorRegistryPath;
            LoadColorValues();
        }


        void LoadColorValues()
        {
            Font = GetFontFromRegistry(FontRegistryPath);
            FontColor = GetColorFromRegistry(FontColorRegistryPath);
        }
    }
}
