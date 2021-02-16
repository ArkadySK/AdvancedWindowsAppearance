using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace AdvancedWindowsAppearence
{
    class AppearanceSetting
    {
        public bool IsEdited;
        public Font Font;
        public bool Font_isBold;
        public bool Font_isItalic;
        public string Name;
        public int FontSize;
        public float? Size;
        readonly string SizeRegistryPath;
        readonly string FontRegistryPath;
        public readonly string ItemColorRegistryPath;
        readonly string AeroColorRegistryPath; //weirdo kod
        public readonly string FontColorRegistryPath;
        string WallpaperPath;
        string Type;
        public Color? FontColor;
        public string FontColorValue;
        public Color? ItemColor;
        public string ItemColorValue;
        readonly int fontNameStartIndex = 28; //odtialto zacina string (nazov fontu) vramci jedneko keyu v registri

        public AppearanceSetting(string _name, string _sizeRegistryPath, string _fontRegistryhPath, string _fontColorRegistryPath, string _colorRegistryPath, string _color2RegistryPath)
        {
            Name = _name;
            FontRegistryPath = _fontRegistryhPath;
            FontColorRegistryPath = _fontColorRegistryPath;
            SizeRegistryPath = _sizeRegistryPath;
            ItemColorRegistryPath = _colorRegistryPath;
            LoadColorValues();
        }

        public AppearanceSetting(string _name, string _regeditPath, string _colorRegistryPath, string _type)
        {
            Name = _name;
            Type = _type;
            switch (Type)
            {
                case "Item":
                    {
                        SizeRegistryPath = _regeditPath;
                        ItemColorRegistryPath = _colorRegistryPath;
                        break;
                    }
                case "Font":
                    {
                        FontRegistryPath = _regeditPath;
                        FontColorRegistryPath = _colorRegistryPath;
                        break;
                    }
            }
            LoadColorValues();
        }

        public AppearanceSetting(string _name, string _aeroColor1RegistryPath, string _aeroColor2RegistryPath)
        {
            Name = _name;
            AeroColorRegistryPath = _aeroColor1RegistryPath;
            DWMSetting dWM = new DWMSetting();
            ItemColor = dWM.GetAeroColorFromRegistry(AeroColorRegistryPath);
            Type = "AeroColor";
        }

        public AppearanceSetting(string _name, string _wallpaperPath) //wallpaper
        {
            Name = _name;
            WallpaperPath = _wallpaperPath;
            if (_wallpaperPath == "")
            {
                WallpaperPath = GetWallpaperPath();
            }
            Type = "Wallpaper";
        }

        public string GetWallpaperPath()
        {
            try
            {
                return Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop").GetValue("Wallpaper").ToString();
            }
            catch
            {
                return "";
            }
        }

        void LoadColorValues()
        {
            Font = GetFontFromRegistry(FontRegistryPath);
            Size = GetSizeFromRegistry(SizeRegistryPath);
            ItemColor = GetColorFromRegistry(ItemColorRegistryPath);
            FontColor = GetColorFromRegistry(FontColorRegistryPath);
        }


        string Color_ToRegistryFormat(Color color)
        {
            return color.R + " " + color.G + " " + color.B;
        }

        public void ConvertColorValuesToRegistry()
        {

            if (Type != "AeroColor")
            {

                if (ItemColor.HasValue)
                {
                    ItemColorValue = Color_ToRegistryFormat(ItemColor.Value);
                }
                if (FontColor.HasValue)
                {
                    FontColorValue = Color_ToRegistryFormat(FontColor.Value);
                }
            }
        }
       
        public void ChangeFontName(string name)
        {
            this.Font = FindFontFromString(name);
            IsEdited = true;
        }

        public void ChangeFontBoldness(bool isBold) //fixni aby to nebolo hneď (až po Apply)
        {
            Font_isBold = isBold;
            IsEdited = true;
        }

        public void ChangeFontItalicness(bool isItalic)
        {
            Font_isItalic = isItalic;
            IsEdited = true;

        }

        public void ChangeSize(float size)
        {
            Size = size;
            IsEdited = true;
        }

        List<Font> GetSystemFonts()
        {
            List<Font> fonts = new List<Font>();

            foreach (FontFamily fontfamily in FontFamily.Families)
            {
                Font f = new Font(fontfamily, 9);
                fonts.Add(f);

            }
            return fonts;
        }

        Font FindFontFromString(string stringname)
        {
            List<Font> fonts = GetSystemFonts();
            foreach (var f in fonts)
            {
                if (stringname.Contains(f.Name))
                {
                    return f;
                }
            }
            Console.WriteLine("Font not found!");
            return null;
        }



        Font GetFontFromRegistry(string regeditpath)
        {
            Font regeditFont;
            if (regeditpath == null || regeditpath == "") return null;

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop\WindowMetrics");

            byte[] fonttemp = (byte[])registryKey.GetValue(regeditpath);
            List<byte> fontNametemp = new List<byte>();
            int i = 0;
            foreach (byte b in fonttemp)
            {
                Console.Write(b + ", ");
                if (i >= fontNameStartIndex && b != 0)
                {
                    fontNametemp.Add(b);
                }
                i++;
            }
            string fontstring = Encoding.ASCII.GetString(fontNametemp.ToArray());
            registryKey.Close();

            regeditFont = FindFontFromString(fontstring);        

            Console.WriteLine(fontstring);
            if (fonttemp[17] == 02)
            {
                Font_isBold = true;
            }
            if (fonttemp[20] == 01)
            {
                Font_isItalic = true;
            }
            int sizetemp = (int)fonttemp[0];
            Console.WriteLine(sizetemp);
            this.FontSize = (256 - sizetemp) / 2;

            return regeditFont;
        }

        int? GetSizeFromRegistry(string regeditpath)
        {
            if (regeditpath == null || regeditpath == "") return null;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            if (registryKey == null) return null;

            var sizeReg = registryKey.GetValue(regeditpath);
            registryKey.Close();
            if (sizeReg == null) return null;

            int intsize = int.Parse(sizeReg.ToString());
            if (regeditpath != "Shell Icon Size")
                intsize = intsize / (-15);

            return intsize;
        }

        Color? GetColorFromRegistry(string regeditpath)
        {
            if (regeditpath == null || regeditpath == "") return null;
            Color color = new Color();
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors");
            if (registryKey == null) return null;

            var colorReg = registryKey.GetValue(regeditpath);
            registryKey.Close();
            if (colorReg == null) return null;

            var colorRegString = colorReg.ToString().Split(' ');
            color = Color.FromArgb(int.Parse(colorRegString[0]), int.Parse(colorRegString[1]), int.Parse(colorRegString[2]));

            return color;
        }

        public void SaveFontToRegistry() /// to do: fixni font 
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            List<byte> regeditValBytes = ((byte[])key.GetValue(FontRegistryPath)).ToList();
            key.Close();

            if (Font_isBold)
                regeditValBytes[17] = 2;
            else
                regeditValBytes[17] = 1;

            if (Font_isItalic)
                regeditValBytes[20] = 1;
            else
                regeditValBytes[20] = 0;
            regeditValBytes.RemoveRange(fontNameStartIndex, regeditValBytes.Count - fontNameStartIndex);

            byte fontsizetemp = (byte)(256 - 2 * this.FontSize);
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
        }


        public void SaveColorsToRegistry()
        {

            if (Type != "AeroColor")
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors", true);
                if (key == null)
                {
                    Registry.CurrentUser.CreateSubKey(@"Control Panel\Colors");
                }

                if (ItemColor.HasValue)
                {
                    key.SetValue(ItemColorRegistryPath, ItemColorValue, RegistryValueKind.String);
                }

                if (FontColor.HasValue)
                {
                    key.SetValue(FontColorRegistryPath, FontColorValue, RegistryValueKind.String);
                }

                key.Close();
            }
        }

        public void SaveSizeToRegistry()
        {
            if (!Size.HasValue) return;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics", true);
            float sizeTemp = -15;

            if (SizeRegistryPath == "Shell Icon Size")
                sizeTemp = Size.Value;
            else
                sizeTemp = -15 * Size.Value;

            key.SetValue(SizeRegistryPath, sizeTemp, RegistryValueKind.String);
            key.Close();
        }
    }
}
