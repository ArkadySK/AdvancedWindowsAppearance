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
