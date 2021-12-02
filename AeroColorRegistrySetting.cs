using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class AeroColorRegistrySetting: RegistrySetting , INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Color? _itemColor;
        public Color? ItemColor { get
            {
                return _itemColor; 
            }
            set 
            {
                _itemColor = value;
                _opacity = _itemColor.GetValueOrDefault().A;
                NotifyPropertyChanged();
            } 
        }
        private byte _opacity;
        public byte Opacity 
        {
            get 
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
                ItemColor = Color.FromArgb(value, ItemColor.GetValueOrDefault(Color.Transparent));
                NotifyPropertyChanged();
            }
        }

        public bool Enabled { get; set; }

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
            if (colorReg == null)
            {
                this.Enabled = false; 
                return Color.Silver;
            }
            try
            {
                color = (Color)(new ColorConverter()).ConvertFromInvariantString(colorReg.ToString());
                this.Enabled = true;
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
            string colorstring = (color.R | (color.G << 8) | (color.B << 16) | (color.A << 24)).ToString();
            return colorstring;
        }
    }
}
