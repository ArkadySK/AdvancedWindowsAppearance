using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class ColorAppearanceSetting : AppearanceSetting
    {
        readonly string SizeRegistryPath;
        public readonly string ColorRegistryPath;
        public Color? ItemColor;
        public string ItemColorValue;

        public ColorAppearanceSetting(string _name, string _regeditPath, string _colorRegistryPath)
        {
            Name = _name;
            SizeRegistryPath = _regeditPath;
            ColorRegistryPath = _colorRegistryPath;
            LoadColorValues();
        }


        void LoadColorValues()
        {
            Size = GetSizeFromRegistry(SizeRegistryPath);
            ItemColor = GetColorFromRegistry(ColorRegistryPath);
        }

    }
}
