using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace AdvancedWindowsAppearence
{
    public class AppearanceSetting
    {
        public bool IsEdited;
        public string Name;
        public float? Size;


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


    }
}
