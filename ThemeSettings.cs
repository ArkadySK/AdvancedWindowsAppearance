using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace AdvancedWindowsAppearence
{
    class ThemeSettings
    {
        public struct ColorRegistrySetting
        {
            public string RegistryName { get; set; }
            public string Value { get; set; }
            public bool IsFoundInTheme{get; set; }
        }

        public string GetThemePath()
        {
            
            string RegistryPath = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes";
            string themepath = (string)Registry.GetValue(RegistryPath, "CurrentTheme", string.Empty);

            /*int ChangeIcons = (int)Registry.GetValue(RegistryKey, "ThemeChangesDesktopIcons", 0);
            int ChangeCursors = (int)Registry.GetValue(RegistryKey, "ThemeChangesMousePointers", 0);

            Registry.SetValue(RegistryKey, "ThemeChangesDesktopIcons", 1);
            Registry.SetValue(RegistryKey, "ThemeChangesMousePointers", 1);*/
            return themepath;
        }

        private string[] GetThemeFile(string themename)
        {
            if (!File.Exists(themename))
            {
                themename = @"C:\Windows\Resources\Themes\aero.theme";
            }
            StreamReader streamReader = new StreamReader(themename);
            string ThemeSettingsIni = streamReader.ReadToEnd();
            streamReader.Close();
            return ThemeSettingsIni.Split(Environment.NewLine.ToCharArray());        
        }

        public ThemeSettings()
        {

        }

        public ThemeSettings(string displayName, Color colorizationColor, string aeroStyle, WallpaperAppearanceSetting wallpaper, ColorAppearanceSetting[] colorAppearanceSettings, FontAppearanceSetting[] fontAppearanceSettings)
        {

            List<ColorRegistrySetting> changedColorSettings = new List<ColorRegistrySetting>();

            string themePath = GetThemePath();
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = themeSettingsIni.ToList();

            int themeIdIndex = -1;
            int colorsIdIndex = -1;
            int desktopIdIndex = -1;
            int visualStylesIdIndex = -1;
            string colorsId = @"[Control Panel\Colors]";
            string visualStylesId = @"[VisualStyles]";
            string desktopId = @"[Control Panel\Desktop]";
            string themeId = @"[Theme]";
            
            //all lines to that contain these will be removed
            string[] headersToRemove = new string[]{
                "Pattern=", "Wallpaper=", "TileWallpaper=", "WallpaperStyle=", "PicturePosition=", "WallpaperWriteTime=",
                "Path=", "ColorizationColor=", "AutoColorization=", "Size=", "ColorStyle",
                "DisplayName=", "ThemeId="};

            foreach (string line in themeSettingsIni)
            {
                //ignore all comments
                if (line.StartsWith(";")) continue;

                //remove all wallpaper lines to avoid mess in the theme
                foreach (string header in headersToRemove)
                    if (line.Contains(header))
                        newThemeSettingsIni.Remove(line);
            }

            //Set theme name
            themeIdIndex = newThemeSettingsIni.IndexOf(themeId);
            if (themeIdIndex == -1)
            {
                newThemeSettingsIni.Add(themeId);
                themeIdIndex = newThemeSettingsIni.Count - 1;
            }
            newThemeSettingsIni.Insert(themeIdIndex + 1, "DisplayName=" + displayName);


            //Set wallpaper
            desktopIdIndex = newThemeSettingsIni.IndexOf(desktopId);
            if (desktopIdIndex == -1)
            {
                newThemeSettingsIni.Add(desktopId);
                desktopIdIndex = newThemeSettingsIni.Count - 1;
            }
            newThemeSettingsIni.Insert(desktopIdIndex + 1, "Wallpaper=" + wallpaper.Path);
            if (wallpaper.WallpaperStyleSettings.SelectedWallpaperStyle == WallpaperStyleRegistrySetting.WallpaperStyle.Tile)
                newThemeSettingsIni.Insert(
                    desktopIdIndex + 2,
                    "TileWallpaper=1\n" +
                    "WallpaperStyle=0");
            else
                newThemeSettingsIni.Insert(
                    desktopIdIndex + 2,
                    "TileWallpaper=0\n" +
                    "WallpaperStyle=" + (int)wallpaper.WallpaperStyleSettings.SelectedWallpaperStyle);
          


            //Set visual styles
            visualStylesIdIndex = newThemeSettingsIni.IndexOf(visualStylesId);
            if (visualStylesIdIndex == -1) {
                newThemeSettingsIni.Add(visualStylesId);
                visualStylesIdIndex = newThemeSettingsIni.Count - 1;
            }
            string visualStylesText =
                "ColorStyle=NormalColor\n" +
                "Size=NormalSize\n" +
                "AutoColorization=0\n";
            if (!string.IsNullOrWhiteSpace(aeroStyle))
                visualStylesText += "Path=" + aeroStyle + "\n";
            if(colorizationColor != null)
            {
                string colorstring = (colorizationColor.B | (colorizationColor.G << 8) | (colorizationColor.R << 16) | (colorizationColor.A << 24)).ToString();
                visualStylesText += "ColorizationColor=" + colorstring + "\n";
            }
            newThemeSettingsIni.Insert(visualStylesIdIndex + 1 ,visualStylesText);
            



            //Set custom colors
            #region ColorSettings

            colorsIdIndex = newThemeSettingsIni.IndexOf(colorsId);
            if (colorsIdIndex == -1)
            {
                newThemeSettingsIni.Add(colorsId);
                colorsIdIndex = newThemeSettingsIni.Count - 1;
            }

            foreach (ColorAppearanceSetting s in colorAppearanceSettings)
            {
                if (s == null) continue;
                if (!s.IsEdited) continue;
                if (s.ItemColor == null) continue;
                
                ColorRegistrySetting cs = new ColorRegistrySetting();
                cs.IsFoundInTheme = false;
                s.ConvertColorValuesToRegistry(s.ItemColor.Value);
                cs.Value = s.ColorRegistryPath + "=" + s.ItemColorValue;
                cs.RegistryName = s.ColorRegistryPath;
                changedColorSettings.Add(cs);
                
            }

            foreach (FontAppearanceSetting s in fontAppearanceSettings)
            {
                if (s == null) continue;
                if (!s.IsEdited) continue;
                if (s.ItemColor == null) continue;

                ColorRegistrySetting cs = new ColorRegistrySetting();
                cs.IsFoundInTheme = false;
                s.ConvertColorValuesToRegistry(s.ItemColor.Value);
                cs.Value = s.ColorRegistryPath + "=" + s.ItemColorValue;
                cs.RegistryName = s.ColorRegistryPath;
                changedColorSettings.Add(cs);
                
            }

            colorsIdIndex = Array.IndexOf(themeSettingsIni, colorsId);

            ///skontroluj ci je tam dany item
            ///output by mal byt true false
            for (int index = 0; index < changedColorSettings.Count; index++)
            {
                int y = 0;
                foreach (string l in themeSettingsIni)
                {
                    if (l.Contains(changedColorSettings[index].RegistryName+"="))
                    {
                        ColorRegistrySetting colorRegistrySetting = new ColorRegistrySetting();
                        colorRegistrySetting.IsFoundInTheme = true;
                        colorRegistrySetting.RegistryName= changedColorSettings[index].RegistryName;
                        colorRegistrySetting.Value = changedColorSettings[index].Value;
                        changedColorSettings[index] = colorRegistrySetting;
                        newThemeSettingsIni[y] = changedColorSettings[index].Value;
                    }
                y++;
                }
            }

            foreach (ColorRegistrySetting cs in changedColorSettings)
            {
                if (!cs.IsFoundInTheme)
                {
                    ///find infex of colorId and insert the rest after it
                    try
                    {
                        newThemeSettingsIni.Insert(colorsIdIndex - 1, cs.Value);

                    }
                    catch
                    {
                        newThemeSettingsIni.Add(colorsId);
                        newThemeSettingsIni.Add(cs.Value);
                    }
                }
            }


            #endregion

            string newTheme = "";
            foreach (string l in newThemeSettingsIni)
            {
                if (l == newThemeSettingsIni[0])
                    newTheme = l;
                else
                {
                    if(l!="")
                    newTheme = newTheme + Environment.NewLine + l;

                }
            }

            string newThemePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Themes\" + displayName + ".theme";
            ///save theme
            SaveTheme(newThemePath, newTheme);
            ///load the new theme
            LoadTheme(newThemePath);
        }

        private void SaveTheme(string themePath, string newThemeSettingsIni)
        {
            if (File.Exists(themePath))
                File.Delete(themePath);
            StreamWriter streamWriter = new StreamWriter(themePath);
            streamWriter.Write(newThemeSettingsIni);
            streamWriter.Close();
        }

        public static void LoadTheme(string themePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            string arg = @"C:\WINDOWS\system32\themecpl.dll,OpenThemeAction " + themePath;
            startInfo.FileName = @"C:\WINDOWS\system32\rundll32.exe";
            startInfo.Arguments = arg;
            process.StartInfo = startInfo;

            process.Start();
            process.Close();
        }
    }
}
