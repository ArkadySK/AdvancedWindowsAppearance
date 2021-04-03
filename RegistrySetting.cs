using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace AdvancedWindowsAppearence
{
    public class RegistrySetting
    {
        public bool? Checked { get; set; }
        public string Name { get; set; }
        public bool IsEdited;
        public string RegistryKey;
        public string RegistryPath;

        public RegistrySetting() { }

        public RegistrySetting(string name, string registrypath, string registrykey)
        {
            Name = name;
            RegistryKey = registrykey;
            RegistryPath = registrypath;
            var val = GetBooleanFromRegistry();
            if (val is int)
                Checked = ((int)val != 0);
            else if (val is bool) Checked = (bool)val;
        }

        public object GetBooleanFromRegistry()
        {
            var val = GetValueFromRegistry();
            if (val == null) val = true;
            return val;
        }

        public object GetValueFromRegistry()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
            if (registryKey == null)
            {
                return null;
            }
            var val = registryKey.GetValue(RegistryKey);
            registryKey.Close();
            return val;
        }

        public void RemoveFromRegistry()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
            if (registryKey == null)
            {
                registryKey.Close();
                return;
            }
            try
            {
                registryKey.DeleteValue(RegistryKey);
            }
            catch { }
        } 

        public void SaveToRegistry()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(RegistryPath, true);
            if (registryKey == null)
            {
                registryKey.Close();
                return;
            }
            registryKey.SetValue(RegistryKey, Checked, RegistryValueKind.DWord);
            registryKey.Close();
            IsEdited = false;
        }

    }
}
