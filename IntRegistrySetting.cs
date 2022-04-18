using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdvancedWindowsAppearence
{
    public class IntRegistrySetting : INotifyPropertyChanged
    {
        private int? _value;
        public int? Value
        {
            get => _value; set
            {
                _value = value;
                NotifyPropertyChanged();
            }
        }

        public string Name { get; set; }
        public string RegistryKey;
        public string RegistryPath;

        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public IntRegistrySetting(string name, string registrypath, string registrykey)
        {
            Name = name;
            RegistryKey = registrykey;
            RegistryPath = registrypath;
            var val = GetValueFromRegistry();

            if (val is int)
                Value = (int)val;
            else
                Value = null;
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
            registryKey.SetValue(RegistryKey, Value, RegistryValueKind.DWord);
            registryKey.Close();
        }

    }
}
