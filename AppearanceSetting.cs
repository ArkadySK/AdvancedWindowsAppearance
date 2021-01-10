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
        public float? Size;
        readonly string SizeRegeditPath;
        readonly string FontRegeditPath;
        public readonly string ItemColorRegeditPath;
        readonly string AeroColorRegeditPath; //weirdo kod
        public readonly string FontColorRegeditPath;
        string WallpaperPath;
        string Type;
        public Color? FontColor;
        public string FontColorValue;
        public Color? ItemColor;
        public string ItemColorValue;

        public AppearanceSetting(string _name, string _sizeRegeditPath, string _fontRegedithPath, string _fontColorRegeditPath, string _colorRegeditPath, string _color2RegeditPath)
        {
            Name = _name;
            FontRegeditPath = _fontRegedithPath;
            FontColorRegeditPath = _fontColorRegeditPath;
            SizeRegeditPath = _sizeRegeditPath;
            ItemColorRegeditPath = _colorRegeditPath;
            LoadColorValues();
        }

        public AppearanceSetting(string _name, string _regeditPath, string _colorRegeditPath, string _type)
        {
            Name = _name;
            Type = _type;
            switch (Type)
            {
                case "Item":
                    {
                        SizeRegeditPath = _regeditPath;
                        ItemColorRegeditPath = _colorRegeditPath;
                        break;
                    }
                case "Font":
                    {
                        FontRegeditPath = _regeditPath;
                        FontColorRegeditPath = _colorRegeditPath;
                        break;
                    }
            }
            LoadColorValues();
        }

        public AppearanceSetting(string _name, string _aeroColor1RegeditPath, string _aeroColor2RegeditPath)
        {
            Name = _name;
            AeroColorRegeditPath = _aeroColor1RegeditPath;          
            ItemColor = GetAeroColorFromRegedit(AeroColorRegeditPath);
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
            Font = GetFontFromRegedit(FontRegeditPath);
            Size = GetSizeFromRegedit(SizeRegeditPath);
            ItemColor = GetColorFromRegedit(ItemColorRegeditPath);
            FontColor = GetColorFromRegedit(FontColorRegeditPath);
        }


        public void SaveFontToRegedit() /// to do: fixni font 
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            byte[] regeditval = (byte[])key.GetValue(FontRegeditPath);
            key.Close();
            //key.Dispose();

            if (Font_isBold)
                regeditval[17] = 02;
            else
                regeditval[17] = 01;

            if (Font_isItalic)
                regeditval[20] = 01;
            else
                regeditval[20] = 00;

            RegistryKey newKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics", true);
            newKey.SetValue(FontRegeditPath, regeditval, RegistryValueKind.Binary);
            newKey.Close();
        }

        string Color_ToRegeditFormat(Color color)
        {
           return color.R + " " + color.G + " " + color.B;
        }

        public void ConvertColorValuesToRegedit()
        {

            if (Type!="AeroColor")
            {

                if (ItemColor.HasValue)
                {
                    ItemColorValue = Color_ToRegeditFormat(ItemColor.Value);
                }
                if (FontColor.HasValue)
                {
                    FontColorValue = Color_ToRegeditFormat(FontColor.Value);
                }
            }
        }

        public void SaveColorsToRegedit()
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
                    key.SetValue(ItemColorRegeditPath, ItemColorValue, RegistryValueKind.String);
                }
               
                if (FontColor.HasValue)
                {
                    key.SetValue(FontColorRegeditPath, FontColorValue, RegistryValueKind.String);
                }

                key.Close();
            }
        }

        public void SaveSizeToRegedit()
        {
            if (!Size.HasValue) return;

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics", true);
            float sizeTemp = -15;

            if (SizeRegeditPath == "Shell Icon Size")
                sizeTemp = Size.Value;
            else
                sizeTemp = -15 * Size.Value;

            key.SetValue(SizeRegeditPath, sizeTemp, RegistryValueKind.String);
            key.Close();
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

            foreach (FontFamily font in FontFamily.Families)
            {
                Font f = new Font(font, 666); //fixni to potom na deviatku
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


        Font GetFontFromRegedit(string regeditpath)
        {
            Font regeditFont;
            if (regeditpath == null || regeditpath == "") return null;

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Control Panel\Desktop\WindowMetrics");
            byte[] fonttemp = (byte[])registryKey.GetValue(regeditpath);          
            string fontstring = Encoding.Unicode.GetString(fonttemp, 0, fonttemp.Count());
            registryKey.Close();

            regeditFont = FindFontFromString(fontstring);
            fontstring = fontstring.Replace(regeditFont.Name, "");
            fontstring = fontstring.Replace(fontstring[0].ToString(), "");
            fontstring = fontstring.Replace("?", "");

            Console.WriteLine(fontstring);
            if (fonttemp[17] == 02)
            {
                Font_isBold = true;
            }
            if (fonttemp[20] == 01)
            {
                Font_isItalic = true;
            }
            /*MemoryStream memoryStream = new MemoryStream(regeditval);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            Console.WriteLine(binaryReader.ReadInt32());
            Console.WriteLine(binaryReader.ReadChars(regeditval.Count()));*/
            return regeditFont;
        } 

        int? GetSizeFromRegedit(string regeditpath)
        {
            if (regeditpath == null || regeditpath == "") return null;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop\WindowMetrics");
            if (registryKey == null) return null;
            int intsize = int.Parse(registryKey.GetValue(regeditpath).ToString());
            if(regeditpath != "Shell Icon Size")
                intsize = intsize / (-15);
            registryKey.Close();
            return intsize;
        }

        Color? GetColorFromRegedit(string regeditpath)
        {
            if (regeditpath == null || regeditpath == "") return null;
            Color color = new Color();
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Control Panel\Colors");
            if (registryKey == null) return null;
            var colorReg = registryKey.GetValue(regeditpath).ToString().Split(' ');
            registryKey.Close();
            color = Color.FromArgb(int.Parse(colorReg[0]), int.Parse(colorReg[1]), int.Parse(colorReg[2]));

            return color;
        }

        Color? GetAeroColorFromRegedit(string regeditpath)
        {
            if (regeditpath == null || regeditpath == "") return null;

            Color color = new Color();
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");
            if (registryKey == null) return null;

            var colorReg = registryKey.GetValue(regeditpath, null);
            if (colorReg == null) return Color.Silver;
            registryKey.Close();
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
    }
}
