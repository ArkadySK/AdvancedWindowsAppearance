using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AdvancedWindowsAppearence
{
    class DWMSetting
    {
        public int Value;
        public bool IsEdited;
        readonly string RegistryPath;

        public DWMSetting() {}

        public DWMSetting(string _registrypath)
        {
            RegistryPath = _registrypath;
            Value = (int)GetValueFromRegistry(RegistryPath);
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
            if (registryKey == null) return null;
            var val = registryKey.GetValue(registrypath);
            registryKey.Close();
            return val;
        }

        public void EditValue(int newVal)
        {
            Value = newVal;
            IsEdited = true;
        }
        
        public void SaveToRegistry()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM", true);
            if (registryKey == null) return;
            registryKey.SetValue(RegistryPath, Value, RegistryValueKind.DWord);
            registryKey.Close();
            IsEdited = false;
        }
    }
}
