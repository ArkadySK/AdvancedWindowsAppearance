using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace AdvancedWindowsAppearence
{
    public class AppearanceSetting : INotifyPropertyChanged
    {
        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isEdited;
        public bool IsEdited { get => _isEdited; set
            {
                if (_isEdited == value) return;
                _isEdited = value;
                NotifyPropertyChanged();
            } 
        }
        public string Name { get; set; }

        private float? _size;
        public float? Size
        {
            get => _size;
            set
            {
                if (_size == value || value == null)
                    return;
                IsEdited = true;
                _size = value;
                if(FontManager.DPI != -1f)              
                    SizeWithDPI = value / (float)FontManager.DPI;
                NotifyPropertyChanged();
            }
        }

        private float? _sizeWithDPI;
        public float? SizeWithDPI {
            get => _sizeWithDPI;
            set
            {
                _sizeWithDPI = value;
                NotifyPropertyChanged();
            }
        }

        public bool HasSize
        {
            get { return Size != null; }
        }
        public string ColorRegistryPath;
        private Color? itemColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public Color? ItemColor
        {
            get => itemColor;
            set
            {
                itemColor = value;
                IsEdited = true;
                NotifyPropertyChanged();
            }
        }
        public string ItemColorValue
        {
            get
            {
                if(!ItemColor.HasValue) return null;
                return ConvertColorValuesToRegistry(ItemColor.Value);
            }
        }

        public bool HasColor
        {
            get
            {
                return ItemColor.HasValue;
            }
        }

        public string ConvertColorValuesToRegistry(Color color)
        {
            return color.R + " " + color.G + " " + color.B;
        }

        internal Color? GetColorFromRegistry(string registrypath)
        {
            if (registrypath == null || registrypath == "") return null;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors");
            if (registryKey == null) return null;

            var colorReg = registryKey.GetValue(registrypath);
            registryKey.Close();
            if (colorReg == null) return null;

            var colorRegString = colorReg.ToString().Split(' ');
            Color color = Color.FromArgb(int.Parse(colorRegString[0]), int.Parse(colorRegString[1]), int.Parse(colorRegString[2]));

            return color;
        }

        internal void SaveColorToRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true);
            if (key == null)
            {
                Registry.CurrentUser.CreateSubKey(@"Control Panel\Colors");
            }

            if (ItemColor.HasValue)
            {
                key.SetValue(ColorRegistryPath, ItemColorValue, RegistryValueKind.String);
            }

            key.Close();
        }
    }
}
