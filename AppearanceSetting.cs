using System;
using System.Drawing;
using Microsoft.Win32;

namespace AdvancedWindowsAppearence
{
    public class AppearanceSetting
    {
        public bool IsEdited { get; set; } = false;
        public string Name { get; set; }
        public float? Size { get; set; }
        public bool HasSize 
        {
            get { return Size != null; }
        }
        public string ColorRegistryPath;

        public Color? ItemColor { get; set; }
        public string ItemColorValue
        {
            get
            {
                return ConvertColorValuesToRegistry(ItemColor);
            }
        }

        public bool HasColor
        {
            get
            {
                return ItemColor.HasValue;
            }
        }

        string Color_ToRegistryFormat(Color color)
        {
            return color.R + " " + color.G + " " + color.B;
        }

        public string ConvertColorValuesToRegistry(Color? color)
        {

            if (color.HasValue)
                return Color_ToRegistryFormat(color.Value);
            return "";
        }
        internal Color? GetColorFromRegistry(string registrypath)
        {
            if (registrypath == null || registrypath == "") return null;
            Color color = new Color();
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors");
            if (registryKey == null) return null;

            var colorReg = registryKey.GetValue(registrypath);
            registryKey.Close();
            if (colorReg == null) return null;

            var colorRegString = colorReg.ToString().Split(' ');
            color = Color.FromArgb(int.Parse(colorRegString[0]), int.Parse(colorRegString[1]), int.Parse(colorRegString[2]));

            return color;
        }
        public void ChangeSize(float size)
        {
            if (Size == size) return;
            Size = size;
            IsEdited = true;
        }

    }
}
