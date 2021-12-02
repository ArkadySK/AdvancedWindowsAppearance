using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class FontAppearanceSetting: AppearanceSetting
    {
        public Font Font { get; set; }
        public string FontName { 
            get 
            {
                return Font.Name;
            } 
        }
        bool isBold;
        public bool IsBold { get
            {
                return isBold;
            }
            set
            {
                isBold = value;
                IsEdited = true;
                base.NotifyPropertyChanged();
            }
        }

        bool isItalic;
        public bool IsItalic 
        {
            get
            {
                return isItalic;
            }
            set
            {
                isItalic = value;
                IsEdited = true;
                base.NotifyPropertyChanged();
            }
        }

        readonly string FontRegistryPath;
        const int fontNameStartIndex = 28; //odtialto zacina string (nazov fontu) vramci jedneko keyu v registri

        public FontAppearanceSetting() { }

        public FontAppearanceSetting(string _name, string _regeditPath, string _colorRegistryPath)
        {
            Name = _name;
            FontRegistryPath = _regeditPath;
            ColorRegistryPath = _colorRegistryPath;
            LoadValues();
        }


        void LoadValues()
        {
            Font = GetFontFromRegistry(FontRegistryPath);
            ItemColor = GetColorFromRegistry(ColorRegistryPath);
        }

        public void ChangeFontName(string name) // verify the way how this works
        {
            this.Font = FontManager.FindFontFromString(name);
            IsEdited = true;
            base.NotifyPropertyChanged();
        }

        internal Font GetFontFromRegistry(string registrypath)
        {
            Font regeditFont;
            if (registrypath == null || registrypath == "") return null;

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop\WindowMetrics");

            byte[] fonttemp = (byte[])registryKey.GetValue(registrypath);
            List<byte> fontNametemp = new List<byte>();
            int i = 0;
            foreach (byte b in fonttemp)
            {
                if (i >= fontNameStartIndex && b != 0)
                {
                    fontNametemp.Add(b);
                }
                i++;
            }
            string fontstring = Encoding.ASCII.GetString(fontNametemp.ToArray());
            registryKey.Close();

            regeditFont = FontManager.FindFontFromString(fontstring);

            if (fonttemp[17] == 02)
            {
                IsBold = true;
            }
            if (fonttemp[20] == 01)
            {
                IsItalic = true;
            }
            int sizetemp = (int)fonttemp[0];
            this.Size = (256 - sizetemp) / 2;

            return regeditFont;
        }

        private void SaveFontToRegistry() 
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            List<byte> regeditValBytes = ((byte[])key.GetValue(FontRegistryPath)).ToList();
            key.Close();

            if (IsBold)
                regeditValBytes[17] = 2;
            else
                regeditValBytes[17] = 1;

            if (IsItalic)
                regeditValBytes[20] = 1;
            else
                regeditValBytes[20] = 0;
            regeditValBytes.RemoveRange(fontNameStartIndex, regeditValBytes.Count - fontNameStartIndex);

            byte fontsizetemp = (byte)(256 - 2 * this.Size);
            regeditValBytes[0] = fontsizetemp;

            List<byte> fontNameBytes = new List<byte>();
            foreach (char c in this.Font.Name)
            {
                byte b = (byte)c;
                fontNameBytes.Add(b);
                fontNameBytes.Add(0);
            }

            List<byte> newRegistryValBytes = regeditValBytes;
            newRegistryValBytes.AddRange(fontNameBytes);
            RegistryKey newKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics", true);
            newKey.SetValue(FontRegistryPath, newRegistryValBytes.ToArray(), RegistryValueKind.Binary);
            newKey.Close();
            IsEdited = false;
        }
        private void SaveColorsToRegistry()
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

        public void SaveToRegistry()
        {
            base.SaveColorToRegistry();
            SaveFontToRegistry();
        }
    }
}
