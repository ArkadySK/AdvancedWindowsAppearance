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
            public string RegeditName { get; set; }
            public string Value { get; set; }
            public bool IsFoundInTheme{get; set; }
        }

        public string GetThemePath()
        {
            string RegistryKey = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes";
            string themepath = (string)Registry.GetValue(RegistryKey, "CurrentTheme", string.Empty);

            /*int ChangeIcons = (int)Registry.GetValue(RegistryKey, "ThemeChangesDesktopIcons", 0);
            int ChangeCursors = (int)Registry.GetValue(RegistryKey, "ThemeChangesMousePointers", 0);

            Registry.SetValue(RegistryKey, "ThemeChangesDesktopIcons", 1);
            Registry.SetValue(RegistryKey, "ThemeChangesMousePointers", 1);*/
            //theme = theme.Split('\\').Last().Split('.').First().ToString();
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
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
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

        public ThemeSettings(string displayName, Color colorizationColor, string aeroStyle, string wallpaperPath, AppearanceSetting[] appearanceSettings)
        {

            List<ColorSetting> changedColorSettings = new List<ColorSetting>();

            string themePath = GetThemePath();
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = themeSettingsIni.ToList();
            List<string> newColorsIni = new List<string>();
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

                if (s.Color1Value != null)
                {
                    ColorSetting cs = new ColorSetting();
                    cs.IsFoundInTheme = false;
                    cs.Value = s.Color1RegeditPath + "=" + s.Color1Value;
                    cs.RegeditName = s.Color1RegeditPath;
                    changedColorSettings.Add(cs);
                }
                if (s.Color2Value != null)
                {
                    ColorSetting cs = new ColorSetting();
                    cs.IsFoundInTheme = false;
                    cs.Value = s.Color2RegeditPath + "=" + s.Color2Value;
                    cs.RegeditName = s.Color2RegeditPath;
                    changedColorSettings.Add(cs);
                }
                if (s.FontColorValue != null)
                {
                    ColorSetting cs = new ColorSetting();
                    cs.IsFoundInTheme = false;
                    cs.Value = s.FontColorRegeditPath + "=" + s.FontColorValue;
                    cs.RegeditName = s.FontColorRegeditPath;
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
                    if (l.Contains(changedColorSettings[index].RegeditName+"="))
                    {
                        ColorSetting colorSetting = new ColorSetting();
                        colorSetting.IsFoundInTheme = true;
                        colorSetting.RegeditName= changedColorSettings[index].RegeditName;
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

            /*
            foreach (var c in changedColorSettings)
            {
                newThemeSettingsIni.Add(c.Value);
            }*/

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
        }
    }
}
