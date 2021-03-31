using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AdvancedWindowsAppearence
{
    public class DWMSetting
    {
        public bool? Checked { get; set; }
        public string Name { get; set; }
        public bool IsEdited;
        readonly string RegistryPath;

        public DWMSetting() {}

        public DWMSetting(string name, string registrypath)
        {
            Name = name;
            RegistryPath = registrypath;
            var val = GetValueFromRegistry(RegistryPath);
            if (val is int)
                Checked = ((int)val != 0);
            else if (val is bool) Checked = (bool)val;
            Console.WriteLine("DWM setting name: " + name + ", regedit value" + val + ", is checked in UI: " + Checked);
        }

        public Color? GetAeroColorFromRegistry(string registrypath)
        {
            if (registrypath == null || registrypath == "") return null;

            Color color = new Color();

            var colorReg = GetValueFromRegistry(registrypath);
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

        object GetValueFromRegistry(string registrypath)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
            if (registryKey == null)
            {
                registryKey.Close();
                return null;
            }
            var val = registryKey.GetValue(registrypath);

            if (val == null) val = true; 
            registryKey.Close();
            return val;
        }
        
        public void SaveToRegistry()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
            if (registryKey == null)
            {
                registryKey.Close();
                return;
            }
            registryKey.SetValue(RegistryPath, Checked, RegistryValueKind.DWord);
            registryKey.Close();
            IsEdited = false;
        }
    }
}
