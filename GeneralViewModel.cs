using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdvancedWindowsAppearence
{
    public class GeneralViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; // to do: implement this to refresh the preview

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ColorAppearanceSetting[] ColorSettings { get; set; }
        public FontAppearanceSetting[] FontSettings { get; set; }

        public WallpaperAppearanceSetting Wallpaper { get; set; } = new WallpaperAppearanceSetting();

        public AeroColorRegistrySetting ThemeColor { get; set; } = new AeroColorRegistrySetting("Theme Color", "ColorizationColor");
        public RegistrySettingsViewModel RegistrySettingsViewModel { get; set; } = new RegistrySettingsViewModel();
        public AeroColorsViewModel AeroColorsViewModel { get; set; } = new AeroColorsViewModel();
        public bool UseThemes = true; //when false: it means to not apply theme, only to change registry settings

        public string ThemeName {get; set;}
    

        public string ThemeStyle = "";

        #region Initialization
        public GeneralViewModel()
        {
            InitAeroColors();
            InitRegistrySettings();
            InitDPIScale();
            InitColors();
            InitFonts();
        }

        void InitAeroColors()
        {
            AeroColorsViewModel.AddNoCheck("Active Window Color", "AccentColor");
            AeroColorsViewModel.AddNoCheck("Inactive Window Color", "AccentColorInactive");
        }

        void InitDPIScale()
        {
            FontManager.GetDPI();
        }

        void InitRegistrySettings()
        {
            RegistrySettingsViewModel.Add("Enable colored titlebars", "ColorPrevalence", new Version(10, 0));
            //RegistrySettingsViewModel.Add("Enable opaque taskbar (win 8 only)", "ColorizationOpaqueBlend", new Version(6, 2));  not working?        
            RegistrySettingsViewModel.Add("Enable peek at desktop", "EnableAeroPeek", new Version(6, 1));
            RegistrySettingsViewModel.Add("Hibernate thumbnails in taskbar", "AlwaysHibernateThumbnails", new Version(6, 1));
            RegistrySettingsViewModel.AddWithPath("Enable transparency effects", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Show accent color on the start and actioncenter", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Use light theme for Apps", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Use light theme for System", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Always hide scrollbars in modern apps", @"Control Panel\Accessibility", "DynamicScrollbars", new Version(10, 0));
        }

        void InitColors()
        {
            ColorSettings = new ColorAppearanceSetting[] { //I -, NI - not implemented, NU - not used
                new ColorAppearanceSetting("Active Title Color 1", "", "ActiveTitle"), //I
                new ColorAppearanceSetting("Active Title Color 2", "", "GradientActiveTitle"), //I
                new ColorAppearanceSetting("Active Title Text", "", "TitleText"), //I
                new ColorAppearanceSetting("Active Window Border", "", "ActiveBorder"), //I
                new ColorAppearanceSetting("Application Background", "", "AppWorkspace"), //NU 
                new ColorAppearanceSetting("Button Alternate Face", "", "ButtonAlternateFace"), // NI - where is it?
                new ColorAppearanceSetting("Button Dark Shadow (Right & Bottom Border)", "", "ButtonDkShadow"),//I
                new ColorAppearanceSetting("Button Face / 3D Objects", "", "ButtonFace"),//I
                new ColorAppearanceSetting("Button Hilight (Edge of Top & Left Border)", "", "ButtonHiLight"), //I       
                new ColorAppearanceSetting("Button Light (Top & Left Border)", "", "ButtonLight"), //I
                new ColorAppearanceSetting("Button Shadow (Edge of Right & Bottom Border)", "", "ButtonShadow"), //I
                new ColorAppearanceSetting("Button Text Color", "", "ButtonText"), //I
                new ColorAppearanceSetting("Caption Buttons Height", "CaptionHeight", ""), //., I
                new ColorAppearanceSetting("Desktop", "", "Background"),//I
                new ColorAppearanceSetting("Gray Text", "", "GrayText"), //I
                new ColorAppearanceSetting("Hilight", "", "Hilight"), //I
                new ColorAppearanceSetting("Hilighted Text", "", "HilightText"),
                new ColorAppearanceSetting("Hypertext link / Hilight (Fill)", "", "HotTrackingColor"), //I
                new ColorAppearanceSetting("Icon Size", "Shell Icon Size", ""),
                new ColorAppearanceSetting("Icon Horizontal Spacing", "IconSpacing", ""),
                new ColorAppearanceSetting("Icon Vertical Spacing", "IconVerticalSpacing", ""),
                new ColorAppearanceSetting("Inactive Title Color 1", "", "InactiveTitle"), //I
                new ColorAppearanceSetting("Inactive Title Color 2", "", "GradientInactiveTitle"), //I
                new ColorAppearanceSetting("Inactive Title Text", "", "InactiveTitleText"), //I
                new ColorAppearanceSetting("Inactive Window Border", "", "InactiveBorder"),
                new ColorAppearanceSetting("Menu", "MenuHeight", "Menu"),//I
                new ColorAppearanceSetting("Scrollbar", "ScrollWidth", "Scrollbar"), //
                new ColorAppearanceSetting("Selected Items", "", "MenuHilight"), //
                new ColorAppearanceSetting("Tool Tip", "", "InfoWindow"), //I
                new ColorAppearanceSetting("Window", "", "Window"),//I
                new ColorAppearanceSetting("Window Border Width", "BorderWidth", ""), //I
                new ColorAppearanceSetting("Window Padded Border", "PaddedBorderWidth", "WindowFrame"), //NU, I
            };
        }

        void InitFonts()
        {
            //TO DO: add font weight and font family bindings
            FontSettings = new FontAppearanceSetting[] {
                new FontAppearanceSetting("Active / Inactive Title Font", "CaptionFont", ""), //., I
                new FontAppearanceSetting("Icon Font", "IconFont", ""),
                new FontAppearanceSetting("Menu Font", "MenuFont", "MenuText"), //I, I
                new FontAppearanceSetting("Palette Title Font", "SmCaptionFont", ""),
                new FontAppearanceSetting("Status Font", "StatusFont", "InfoText"),//I, I
                new FontAppearanceSetting("Window Text Font", "MessageFont", "WindowText") //I, I
            };
        }
        #endregion

        void KillDWM()
        {
            var processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.ProcessName == "dwm")
                {
                    try
                    {
                        p.Kill();
                    }
                    catch
                    {
                        Console.WriteLine("not poss to kill dwm.exe, sry");
                    }
                }
            }
        }

        public void UpdateThemeStyle(string style)
        {
            if (string.IsNullOrWhiteSpace(style))
            {
                ThemeStyle = "";
                return;
            }
            ThemeStyle = @"%SystemRoot%\resources\Themes\" + style + ".msstyles";
        }

        public void ShowThemesControlPanel()
        {
            Process.Start("explorer", "shell:::{ED834ED6-4B5A-4bfe-8F11-A626DCB6A921}");
        }

        public void OpenThemesFolder()
        {
            Process.Start("explorer", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Microsoft\\Windows\\Themes");
        }

        #region Save
        public async Task SaveChanges()
        {
            if (UseThemes)
                await SaveToTheme();  
            else
                await SaveToRegistry();

            await RegistrySettingsViewModel.SaveAll();
            await AeroColorsViewModel.SaveAll();
            //KillDWM(); //not needed?
            MessageBox.Show("You need to restart to apply these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        async Task SaveToRegistry()
        {
            List<Task> tasks = new List<Task>();
            foreach (var c in ColorSettings)
            {
                if (c == null) continue;
                tasks.Add(Task.Run(() => c.SaveToRegistry()));
                c.IsEdited = false;
            }
            foreach (var f in FontSettings)
            {
                if (f == null) continue;
                tasks.Add(Task.Run(() => f.SaveToRegistry()));
                f.IsEdited = false;
            }
            await Task.WhenAll(tasks);
        }

        async Task SaveToTheme()
        {
            string wallpaperPath = Wallpaper.Path; //make UI for this

            if (!UseThemes) return; //if user does not want to save changes into theme

            ThemeSettings SaveTheme = Task.Run(() => new ThemeSettings(ThemeName, ThemeColor.ItemColor.Value, ThemeStyle, wallpaperPath, ColorSettings, FontSettings)).Result;

            foreach (var setting in ColorSettings)
            {
                if (setting == null) continue;
                if (!setting.IsEdited) continue;
                setting.SaveToRegistry();
                setting.IsEdited = false;
            }
            foreach (var f in FontSettings)
            {
                if (f == null) continue;
                if (!f.IsEdited) continue;
                f.SaveToRegistry();
                f.IsEdited = false;
            }

            App.Current.Resources["ThemeColor"] = Converters.BrushToColorConverter.MediaColorToBrush(ThemeColor.ItemColor);
            await Task.Delay(2000);
        }
        #endregion

        #region Restore

        public void RunRegFile(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "reg.exe";
            startInfo.Arguments = "IMPORT \"" + path + "\"";
            Process.Start(startInfo);
        }

        internal async Task ResetFonts()
        {
            RunRegFile("Window Metrics fix.reg");
            await Task.Delay(100);
        }

        internal async Task ResetColors()
        {
            RunRegFile("Colors fix.reg");
            await Task.Delay(100);
        }

        internal async Task ResetDWM()
        {
            RunRegFile("DWM fix.reg");
            await Task.Delay(100);
        }

        internal async Task ResetToDefaults()
        {
            var taskList = new List<Task>();
            taskList.Add(ResetFonts());
            taskList.Add(ResetColors());
            taskList.Add(ResetDWM());
            await Task.WhenAll(taskList);
            await ResetTheme();
        }
        internal async Task ResetTheme()
        {
            ThemeSettings.LoadTheme(@"C:\Windows\Resources\Themes\aero.theme");
            await Task.Delay(1000);
        }
        #endregion

        #region Export

        string GetColorsInReg()
        {
            string output = @"[HKEY_CURRENT_USER\Control Panel\Colors]";

            foreach(var c in ColorSettings)
            {
                if(!c.HasColor)
                    continue;
                output += "\n\"" + c.ColorRegistryPath + "\"=\"" + c.ItemColorValue + "\"";
                }

            output += Environment.NewLine + Environment.NewLine;
            output += @"[HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics]";

            foreach (var c in ColorSettings)
            {
                if (!c.HasSize)
                    continue;
                output += "\n\"" + c.SizeRegistryPath + "\"=\"" + (-15 * c.Size) + "\"";
            }

            return output;
        }
        string GetFontsInReg()
        {
            string output = "";

            foreach (var f in FontSettings)
            {
                if (!f.HasSize)
                    continue;
                string valueText = "hex:";
                foreach(byte b in f.GetFontInRegistryFormat())
                {
                    valueText += b.ToString("X") + ",";
                }
                valueText = valueText.Remove(valueText.Length - 1, 1);
                output += "\n\"" + f.FontRegistryPath + "\"=" + valueText;
            }

            output += Environment.NewLine + Environment.NewLine;
            output += @"[HKEY_CURRENT_USER\Control Panel\Colors]";
            foreach (var f in FontSettings)
            {
                if (!f.HasColor)
                    continue;
                output += "\n\"" + f.ColorRegistryPath + "\"=\"" + f.ItemColorValue + "\"";
            }

            output += Environment.NewLine + Environment.NewLine;
            return output;
        }

        public async Task ExportToReg()
        {
            string textToSave = "Windows Registry Editor Version 5.00\n\n";
            textToSave += GetColorsInReg();
            textToSave += GetFontsInReg();
            textToSave += RegistrySettingsViewModel.GetSettingsInReg();
            textToSave += AeroColorsViewModel.GetSettingsInReg();
            Console.WriteLine(textToSave); //debugging solution

            string path = Environment.CurrentDirectory + "\\Exported Settings";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = "Export" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".reg";

            var sw = new StreamWriter(path + "\\" + fileName);
            await sw.WriteAsync(textToSave);
            sw.Close();
        }

        #endregion

    }
}
