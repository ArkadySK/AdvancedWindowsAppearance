using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    class AeroColorRegistrySetting: RegistrySetting
    {
        public Color? ItemColor { get; set; }

        public AeroColorRegistrySetting(string name, string registrypath, string registrykey)
        {
            Name = name;
            RegistryKey = registrykey;
            if (string.IsNullOrEmpty(registrypath)) RegistryPath = @"Software\Microsoft\Windows\DWM";
            else   RegistryPath = registrypath;
            ItemColor = GetAeroColorFromRegistry(RegistryKey);
            Console.WriteLine(ItemColor.Value);
        }

        public Color? GetAeroColorFromRegistry(string registrykey)
        {
            if (registrykey == null || registrykey == "") return null;

            Color color = new Color();

            var colorReg = GetValueFromRegistry();
            if (colorReg == null) return Color.Silver;
            try
            {
                color = (Color)(new ColorConverter()).ConvertFromInvariantString(colorReg.ToString());
            }
            catch
            {
                return null;
            }
            return color;
        }

        public new void SaveToRegistry()
        {
            if (!ItemColor.HasValue) return;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
            if (registryKey == null)
            {
                Registry.CurrentUser.CreateSubKey(RegistryPath);
            }
            registryKey.SetValue(RegistryKey, Color_ConvertToRegistryFormat(ItemColor.Value), RegistryValueKind.DWord);
            registryKey.Close();
            return;
        }

        string Color_ConvertToRegistryFormat(Color color)
        {
            string colorstring = (color.B | (color.G << 8) | (color.R << 16) | (color.A << 24)).ToString();
            return colorstring;
        }
    }
}
