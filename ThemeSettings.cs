using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AdvancedWindowsAppearence
{
    public class ThemeSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private readonly string _initialThemePath;
        public string ThemeName { get; set; }

        private string _themeStyle = "";
        private string _initialThemeStyle;


        private readonly string _resourcesThemesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "resources", "Themes") + "\\";
        public string ThemeStyle 
        { 
            get => _themeStyle; 
            set
            {
                if (File.Exists(value))
                    _themeStyle = value;
                else if (File.Exists(_resourcesThemesFolder + value))
                    _themeStyle = _resourcesThemesFolder + value;
                else if (File.Exists(_resourcesThemesFolder + value + ".msstyles"))
                    _themeStyle = _resourcesThemesFolder + value + ".msstyles";
                else
                    _themeStyle = "";
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsUsingCustomStyle");
            }
        }

        public bool IsThemeStyleModified
        {
            get => ThemeStyle == _initialThemeStyle;
        }

        public bool IsUsingCustomStyle 
        { 
            get
            => (ThemeStyle != _resourcesThemesFolder + "Aero\\Aero.msstyles" && ThemeStyle != _resourcesThemesFolder + "Aero\\AeroLite.msstyles");
        }

        public GeneralViewModel Settings { get; set; }


        /// <summary>
        /// intialize the ThemeSettings with the viewmodel 
        /// </summary>
        /// <param name="settings"></param>
        public ThemeSettings(GeneralViewModel settings)
        {
            _initialThemePath = GetThemePath();
            ThemeName = GetShortThemeName(_initialThemePath);

            _initialThemeStyle = GetThemeStyle(_initialThemePath);
            _themeStyle = _initialThemeStyle;

            Settings = settings;
        }

        private string GetShortThemeName(string themePath)
        {
            if (string.IsNullOrEmpty(themePath))
                return "New";

            themePath = Path.GetFileNameWithoutExtension(themePath);

            if (!themePath.Contains(" (Edited)"))
                return themePath + " (Edited)";
            else
                return themePath;
        }

        private string GetThemeStyle(string themeName)
        {
            try
            {
                string[] fileLines = GetThemeFile(themeName);

                foreach (var line in fileLines) 
                {
                    if (!line.Contains("Path="))
                        continue;
                    string themeStyle = line.Replace("Path=", "");
                    if (themeStyle.Contains("%ResourceDir%\\Themes\\"))
                        return themeStyle.Replace("%ResourceDir%\\Themes\\", _resourcesThemesFolder);
                    if (themeStyle.Contains("%SystemRoot%\\resources\\Themes\\")) 
                        return themeStyle.Replace("%SystemRoot%\\resources\\Themes\\", _resourcesThemesFolder);
                    return themeStyle;
                }

            }
            catch
            {
                return _resourcesThemesFolder + "Aero\\Aero";
            }
            return "Aero\\Aero";
        }

        public void RestoreThemeStyle() 
            => _themeStyle = _initialThemeStyle;

        public static string GetThemePath()
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
                themename = _resourcesThemesFolder + "aero.theme";
            }
            StreamReader streamReader = new StreamReader(themename);
            string ThemeSettingsIni = streamReader.ReadToEnd();
            streamReader.Close();
            return ThemeSettingsIni.Split(Environment.NewLine.ToCharArray());
        }

        private void VerifyAndUpdateThemeName() 
        {
            string themePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Themes\" + ThemeName + ".theme";
            if (File.Exists(themePath))
            {
                File.Delete(themePath);
                if (ThemeName.EndsWith("_"))
                    ThemeName = ThemeName.Remove(ThemeName.Length - 1, 1);
                else
                    ThemeName += "_";
            }
        }

        #region Save (each type separately)
        readonly string colorsId = @"[Control Panel\Colors]";
        readonly string visualStylesId = @"[VisualStyles]";
        readonly string desktopId = @"[Control Panel\Desktop]";
        readonly string themeId = @"[Theme]";

        public async Task SaveTitleColors()
        {
            string themePath = GetThemePath();
            int visualStylesIdIndex = -1;
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = themeSettingsIni.ToList();

            string[] headersToRemove = new string[]{
                "ColorizationColor=", "AutoColorization=", "ColorStyle", "Size="};

            //remove lines that will be replaced
            var removeLinesTasks = new List<Task>();
            foreach (string line in themeSettingsIni)
            {
                //ignore all comments
                if (line.StartsWith(";")) continue;

                //avoid mess in the theme
                foreach (string header in headersToRemove)
                    if (line.Contains(header))
                        removeLinesTasks.Add(Task.Run(() => newThemeSettingsIni.Remove(line)));
            }
            await Task.WhenAll(removeLinesTasks);

            //Set visual styles and aero color
            visualStylesIdIndex = newThemeSettingsIni.IndexOf(visualStylesId);
            if (visualStylesIdIndex == -1)
            {
                await Task.Run(() => newThemeSettingsIni.Add(visualStylesId));
                visualStylesIdIndex = newThemeSettingsIni.Count - 1;
            }
            var themeColor = Settings.ThemeColor.ItemColor.GetValueOrDefault(Color.Silver);

            StringBuilder themeSb = new StringBuilder();
            themeSb.Append(
                "ColorStyle=NormalColor\n" +
                "Size=NormalSize\n" +
                "AutoColorization=0\n");
            if (themeColor != null)
            {
                string colorstring = (themeColor.B | (themeColor.G << 8) | (themeColor.R << 16) | (themeColor.A << 24)).ToString();
                themeSb.AppendLine("ColorizationColor=" + colorstring);
            }
            await Task.Run(() => newThemeSettingsIni.Insert(visualStylesIdIndex + 1, themeSb.ToString()));

            await SaveTheme(newThemeSettingsIni);
            await LoadCurrentTheme();

        }

        public async Task SaveTheme()
        {
            string themePath = GetThemePath(); ;
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = themeSettingsIni.ToList();
            string[] headersToRemove = new string[]{
                "Path=",
                "DisplayName=", "ThemeId=",
                "SystemMode=", "AppMode="
            };

            // Update theme name
            VerifyAndUpdateThemeName();

            //remove lines that will be replaced
            var removeLinesTasks = new List<Task>();
            foreach (string line in themeSettingsIni)
            {
                //ignore all comments
                if (line.StartsWith(";")) continue;

                //avoid mess in the theme
                foreach (string header in headersToRemove)
                    if (line.Contains(header))
                        removeLinesTasks.Add(Task.Run(() => newThemeSettingsIni.Remove(line)));
            }
            await Task.WhenAll(removeLinesTasks);

            //Set theme name
            int themeIdIndex = newThemeSettingsIni.IndexOf(themeId);
            if (themeIdIndex == -1)
            {
                newThemeSettingsIni.Add(themeId);
                themeIdIndex = newThemeSettingsIni.Count - 1;
            }
            await Task.Run(() => newThemeSettingsIni.Insert(themeIdIndex + 1, "DisplayName=" + ThemeName));

            //Set visual style
            int visualStylesIdIndex = newThemeSettingsIni.IndexOf(visualStylesId);
            if (visualStylesIdIndex == -1)
            {
                newThemeSettingsIni.Add(visualStylesId);
                visualStylesIdIndex = newThemeSettingsIni.Count - 1;
            }

            if (IsThemeStyleModified)
            {
            if (string.IsNullOrWhiteSpace(ThemeStyle))
                    ThemeStyle = _initialThemeStyle;
            }

            var visualStylesText = "Path=" + ThemeStyle;
            newThemeSettingsIni.Insert(visualStylesIdIndex + 1, visualStylesText);

            await SaveTheme(newThemeSettingsIni);
            await LoadCurrentTheme();
        }

        public async Task SaveColorsAndMetrics()
        {
            string themePath = GetThemePath();
            int colorsIdIndex = -1;
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = GetThemeFile(themePath).ToList();

            //remove lines that will be replaced
            var removeLinesTasks = new List<Task>();
            foreach (string line in themeSettingsIni)
            {
                //ignore all comments
                if (line.StartsWith(";")) continue;

                //avoid mess in the theme
                foreach (ColorAppearanceSetting color in Settings.ColorSettings)
                {
                    if (!color.IsEdited)
                        continue;
                    if (line.Contains(color.Name))
                        removeLinesTasks.Add(Task.Run(() => newThemeSettingsIni.Remove(line)));
                }
            }
            await Task.WhenAll(removeLinesTasks);

            // If colorsId was not found
            colorsIdIndex = newThemeSettingsIni.IndexOf(colorsId);
            if (colorsIdIndex == -1)
                newThemeSettingsIni.Add(colorsId);

            foreach (ColorAppearanceSetting color in Settings.ColorSettings)
            {
                if (!color.IsEdited)
                    continue;
                color.IsEdited = false;
                colorsIdIndex = newThemeSettingsIni.IndexOf(colorsId);
                if (color.HasColor)
                    newThemeSettingsIni.Insert(colorsIdIndex + 1, color.ColorRegistryPath + "=" + color.ItemColorValue);
            }

            await SaveTheme(newThemeSettingsIni);
            await LoadCurrentTheme();
        }

        public async Task SaveFonts()
        {
            string themePath = GetThemePath();
            int colorsIdIndex = -1;
            string[] themeSettingsIni = GetThemeFile(themePath);
            List<string> newThemeSettingsIni = GetThemeFile(themePath).ToList();

            // If colorsId was not found
            colorsIdIndex = newThemeSettingsIni.IndexOf(colorsId);
            if (colorsIdIndex == -1)
                newThemeSettingsIni.Add(colorsId);

            //remove lines that will be replaced
            var removeLinesTasks = new List<Task>();
            foreach (string line in themeSettingsIni)
            {
                //ignore all comments
                if (line.StartsWith(";")) continue;

                //avoid mess in the theme
                foreach (FontAppearanceSetting color in Settings.FontSettings)
                {
                    if (!color.IsEdited)
                        continue;
                    if (line.Contains(color.Name))
                        removeLinesTasks.Add(Task.Run(() => newThemeSettingsIni.Remove(line)));
                }
            }
            await Task.WhenAll(removeLinesTasks);

            foreach (FontAppearanceSetting font in Settings.FontSettings)
            {
                if (!font.IsEdited)
                    continue;
                font.IsEdited = false;
                colorsIdIndex = newThemeSettingsIni.IndexOf(colorsId);
                if (font.HasColor)
                    newThemeSettingsIni.Insert(colorsIdIndex + 1, font.Name + "=" + font.ItemColorValue);
            }

            await SaveTheme(newThemeSettingsIni);
            await LoadCurrentTheme();
        }

        public async Task SaveWallpaper()
        {
            //all lines to that contain these will be removed
            string[] headersToRemove = new string[] {
                "Pattern=", "Wallpaper=", "TileWallpaper=", "WallpaperStyle=", "PicturePosition=", "WallpaperWriteTime=", "MultimonBackgrounds",
            };

            string[] themeSettingsIni = GetThemeFile(GetThemePath());
            List<string> newThemeSettingsIni = themeSettingsIni.ToList();

            //remove lines that will be replaced
            var removeLinesTasks = new List<Task>();
            foreach (string line in themeSettingsIni)
            {
                //ignore all comments
                if (line.StartsWith(";")) continue;

                //remove all wallpaper lines to avoid mess in the theme
                foreach (string header in headersToRemove)
                    if (line.Contains(header))
                        removeLinesTasks.Add(Task.Run(() => newThemeSettingsIni.Remove(line)));
            }
            await Task.WhenAll(removeLinesTasks);

            int desktopIdIndex = newThemeSettingsIni.IndexOf(desktopId);
            if (desktopIdIndex == -1)
            {
                newThemeSettingsIni.Add(desktopId);
                desktopIdIndex = newThemeSettingsIni.Count - 1;
            }
            newThemeSettingsIni.Insert(desktopIdIndex + 1, "Wallpaper=" + Settings.WallpaperSettings.Wallpaper.Path);
            if (Settings.WallpaperSettings.Wallpaper.WallpaperStyleSettings.SelectedWallpaperStyle == WallpaperStyleRegistrySetting.WallpaperStyle.Tile)
                newThemeSettingsIni.Insert(
                    desktopIdIndex + 2,
                    "TileWallpaper=1\n" +
                    "WallpaperStyle=0");
            else
                newThemeSettingsIni.Insert(
                    desktopIdIndex + 2,
                    "TileWallpaper=0\n" +
                    "WallpaperStyle=" + (int)Settings.WallpaperSettings.Wallpaper.WallpaperStyleSettings.SelectedWallpaperStyle);

            await SaveTheme(newThemeSettingsIni);
            await LoadCurrentTheme();
        }
        #endregion

        private async Task SaveTheme(List<string> newThemeSettingsIni)
        {
            string themePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Themes\" + ThemeName + ".theme";
            if (File.Exists(themePath))
                File.Delete(themePath);

            StreamWriter streamWriter = new StreamWriter(themePath);
            StringBuilder stringBuilderTheme = new StringBuilder();
            foreach (string line in newThemeSettingsIni)
            {
                if(!string.IsNullOrEmpty(line)) 
                    stringBuilderTheme.AppendLine(line);
            }
            await streamWriter.WriteAsync(stringBuilderTheme.ToString());
            streamWriter.Close();
        }

        private async Task LoadCurrentTheme()
        {
            string themePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Themes\" + ThemeName + ".theme";
            await LoadTheme(themePath);
        }

        public static async Task LoadTheme(string themePath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            string arg = @"C:\WINDOWS\system32\themecpl.dll,OpenThemeAction " + themePath;
            startInfo.FileName = @"C:\WINDOWS\system32\rundll32.exe";
            startInfo.Arguments = arg;
            process.StartInfo = startInfo;

            process.Start();
            await Task.Delay(1000);
            process.Close();
        }
    }
}
