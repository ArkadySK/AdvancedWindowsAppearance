using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class FontAppearanceSetting : AppearanceSetting
    {

        private bool isItalic;
        public Font Font { get => _font; set
            {
                if (_font == value) return;
                _font = value;
                base.NotifyPropertyChanged();
            } 
        }
        public string FontName
        {
            get
            {
                return Font.Name;
            }
        }
        bool isBold;
        public bool IsBold
        {
            get
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

        private Font _font;

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

        public readonly string FontRegistryPath;
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
            IsEdited = false;
        }

        public void ChangeFontName(string name) // verify the way how this works
        {
            if (FontName == name) return;
            this.Font = FontManager.FindFontFromString(name);
            IsEdited = true;
        }

        internal Font GetFontFromRegistry(string registrypath)
        {
            Font regeditFont = new Font(FontFamily.GenericSansSerif, 11);
            if (registrypath == null || registrypath == "") return null;

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop\WindowMetrics");

            try { 

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
                this.Size = (256 - sizetemp) / (float)FontManager.DPI;
            }
            catch
            {
                Console.WriteLine("Unable to load font '{0}'", Name);
            }
            return regeditFont;
        }

        public byte[] GetFontInRegistryFormat()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            List<byte> registryValBytes = ((byte[])key.GetValue(FontRegistryPath)).ToList();
            key.Close();

            if (IsBold)
                registryValBytes[17] = 2;
            else
                registryValBytes[17] = 1;

            if (IsItalic)
                registryValBytes[20] = 1;
            else
                registryValBytes[20] = 0;
            registryValBytes.RemoveRange(fontNameStartIndex, registryValBytes.Count - fontNameStartIndex);

            byte fontsizetemp = (byte)((256 - Size * FontManager.DPI));
            registryValBytes[0] = fontsizetemp;

            List<byte> fontNameBytes = new List<byte>();
            foreach (char c in this.Font.Name)
            {
                byte b = (byte)c;
                fontNameBytes.Add(b);
                fontNameBytes.Add(0);
            }

            List<byte> newRegistryValBytes = registryValBytes;
            newRegistryValBytes.AddRange(fontNameBytes);
            return newRegistryValBytes.ToArray();
        }

        private void SaveFontToRegistry()
        {
            RegistryKey newKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics", true);
            newKey.SetValue(FontRegistryPath, GetFontInRegistryFormat(), RegistryValueKind.Binary);
            newKey.Close();
        }

        public void SaveToRegistry()
        {
            base.SaveColorToRegistry();
            SaveFontToRegistry();
            IsEdited = false;
        }
    }
}
