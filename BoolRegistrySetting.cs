using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdvancedWindowsAppearence
{
    public class BoolRegistrySetting : INotifyPropertyChanged
    {
        private bool? _checked;
        public bool? Checked { get => _checked; set
            {
                _checked = value;
                NotifyPropertyChanged();
            }
        }

        public string Name { get; set; }

        public bool IsEnabled { get; set; } = false;

        public string RegistryKey;
        public string RegistryPath;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BoolRegistrySetting() { }

        public BoolRegistrySetting(string name, string registrypath, string registrykey)
        {
            Name = name;
            RegistryKey = registrykey;
            RegistryPath = registrypath;
            var val = GetBooleanFromRegistry();
            if (val is string)
                Checked = (int.Parse(val.ToString()) != 0);
            if (val is int)
                Checked = ((int)val != 0);
            else if (val is bool) 
                Checked = (bool)val;
            IsEnabled = true;
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
                return;
            registryKey.SetValue(RegistryKey, Checked, RegistryValueKind.DWord);
            registryKey.Close();
        }

    }
}
