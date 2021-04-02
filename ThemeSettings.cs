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
        public struct ColorSetting
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
            StreamReader streamReader = new StreamReader(themename);
            string ThemeSettingsIni = streamReader.ReadToEnd();
            streamReader.Close();
            return ThemeSettingsIni.Split(Environment.NewLine.ToCharArray());        
        }

        private void SaveTheme(string themePath, string newThemeSettingsIni)
        {       
            StreamWriter streamWriter = new StreamWriter(themePath);
            streamWriter.Write(newThemeSettingsIni);
            streamWriter.Close();
        }

        void LoadNewTheme(string themePath)
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

        public ThemeSettings()
        {

        }

        public ThemeSettings(string displayName, Color colorizationColor, string aeroStyle, string wallpaperPath, AppearanceSetting[] itemSettings, AppearanceSetting[] fontSettings)
        {

            List<ColorSetting> changedColorSettings = new List<ColorSetting>();
            var aa = itemSettings.ToList();
            aa.AddRange(fontSettings);

            AppearanceSetting[] appearanceSettings = aa.ToArray();
            string themePath = GetThemePath();
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = themeSettingsIni.ToList();

            int i = 0;
            int ColorIdIndex = -1; ///nech ho to bugne xd
            string colorsId = @"[Control Panel\Colors]";
            if (!themeSettingsIni.Contains(colorsId))
            {
                newThemeSettingsIni.Add(colorsId);
            }

            
            foreach (string line in themeSettingsIni)
            {
                if (line.Contains("DisplayName=") && !string.IsNullOrEmpty(displayName))
                    newThemeSettingsIni[i] = "DisplayName=" + displayName;
                else if (line.Contains("Path=") && !string.IsNullOrEmpty(aeroStyle))
                    newThemeSettingsIni[i] = "Path=" + aeroStyle;
                else if (line.Contains("ColorizationColor=") && colorizationColor!=null)
                {
                    string colorstring = (colorizationColor.B | (colorizationColor.G << 8) | (colorizationColor.R << 16) | (colorizationColor.A << 24)).ToString();
                    newThemeSettingsIni[i] = "ColorizationColor=" + colorstring;
                }
                i++;
            }
            
            #region ColorSettings
            foreach (var s in appearanceSettings)
            {
                if (s == null) continue;
                if (!s.IsEdited) continue;

                if (s.ItemColor != null)
                {
                    ColorSetting cs = new ColorSetting();
                    cs.IsFoundInTheme = false;
                    s.ConvertColorValuesToRegistry();
                    cs.Value = s.ItemColorRegistryPath + "=" + s.ItemColorValue;
                    cs.RegistryName = s.ItemColorRegistryPath;
                    changedColorSettings.Add(cs);
                }
                if (s.FontColor != null)
                {
                    ColorSetting cs = new ColorSetting();
                    cs.IsFoundInTheme = false;
                    s.ConvertColorValuesToRegistry();
                    cs.Value = s.FontColorRegistryPath + "=" + s.FontColorValue;
                    cs.RegistryName = s.FontColorRegistryPath;
                    changedColorSettings.Add(cs);
                }
            }

            ColorIdIndex = Array.IndexOf(themeSettingsIni, colorsId);

            ///skontroluj ci je tam dany item
            ///output by mal byt true false
            for (int index = 0; index < changedColorSettings.Count; index++)
            {
                int y = 0;
                foreach (string l in themeSettingsIni)
                {
                    if (l.Contains(changedColorSettings[index].RegistryName+"="))
                    {
                        ColorSetting colorSetting = new ColorSetting();
                        colorSetting.IsFoundInTheme = true;
                        colorSetting.RegistryName= changedColorSettings[index].RegistryName;
                        colorSetting.Value = changedColorSettings[index].Value;
                        changedColorSettings[index] = colorSetting;
                        newThemeSettingsIni[y] = changedColorSettings[index].Value;
                    }
                y++;
                }
            }

            foreach (ColorSetting cs in changedColorSettings)
            {
                if (!cs.IsFoundInTheme)
                {
                    ///zisti index colorid a insertni ho za neho
                    newThemeSettingsIni.Insert(ColorIdIndex-1, cs.Value);
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
            ///ulozenie temy
            SaveTheme(newThemePath, newTheme);
            ///nacitanie temy
            LoadNewTheme(newThemePath);

            //nacitanie fontov
            foreach(var f in fontSettings)
            {
                if (f != null)
                    f.SaveFontToRegistry();
            }
        }
    }
}
