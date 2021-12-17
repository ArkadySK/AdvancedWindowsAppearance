using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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
        public double DPI = 1;

        public bool UseThemes = true; //when false: it means to not apply theme, only to change registry settings

        public string ThemeName {get; set;}
    

        public string ThemeStyle = "";

        #region Initialization
        public GeneralViewModel()
        {
            InitAeroColors();
            InitRegistrySettings();
            InitColors();
            InitFonts();
            InitDPIScale();
        }

        void InitAeroColors()
        {
            AeroColorsViewModel.AddNoCheck("Active Window Color", "AccentColor");
            AeroColorsViewModel.AddNoCheck("Inactive Window Color", "AccentColorInactive");
        }

        void InitDPIScale()
        {
            DPI = System.Windows.SystemFonts.CaptionFontSize / FontSettings[0].Size.GetValueOrDefault(1f);
        }

        void InitRegistrySettings()
        {
            RegistrySettingsViewModel.Add("Show accent color on the title bars", "ColorPrevalence", new Version(10, 0));
            RegistrySettingsViewModel.Add("Enable opaque taskbar (win 8 only)", "ColorizationOpaqueBlend", new Version(6, 2));
            //RegistrySettingsController.Add("Enable composition", "Composition", new Version(6, 1));           
            RegistrySettingsViewModel.Add("Enable peek at desktop", "EnableAeroPeek", new Version(6, 1));
            RegistrySettingsViewModel.Add("Hibernate Thumbnails", "AlwaysHibernateThumbnails", new Version(6, 1));
            RegistrySettingsViewModel.AddWithPath("Enable transparency effects", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Show accent color on the start and actioncenter", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Apps use light theme", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("System uses light theme", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", new Version(10, 0));
            RegistrySettingsViewModel.AddWithPath("Always show scrollbars in modern apps", @"Control Panel\Accessibility", "DynamicScrollbars", new Version(10, 0));
        }

        void InitColors()
        {
            ColorSettings = new ColorAppearanceSetting[] { //I -, NI - not implemented (new way how to do so), to do, NU - not used
                new ColorAppearanceSetting("Active Title Color 1", "", "ActiveTitle"), //I
                new ColorAppearanceSetting("Active Title Color 2", "", "GradientActiveTitle"), //I
                new ColorAppearanceSetting("Active Title Text", "", "TitleText"), //I
                new ColorAppearanceSetting("Active Window Border", "", "ActiveBorder"), //I
                new ColorAppearanceSetting("Application Background", "", "AppWorkspace"), //NU
                new ColorAppearanceSetting("Button Alternate Face", "", "ButtonAlternateFace"), // NI
                new ColorAppearanceSetting("Button Dark Shadow (Right & Bottom Border)", "", "ButtonDkShadow"),//I
                new ColorAppearanceSetting("Button Face / 3D Objects", "", "ButtonFace"),//I, not applied on scrollbar
                new ColorAppearanceSetting("Button Light (Top & Left Border)", "", "ButtonLight"), //I
                new ColorAppearanceSetting("Button Shadow Color", "", "ButtonShadow"), //NI
                new ColorAppearanceSetting("Button Text Color", "", "ButtonText"), //I
                new ColorAppearanceSetting("Caption Buttons Height", "CaptionHeight", ""),
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
                new ColorAppearanceSetting("Scrollbar", "ScrollWidth", "Scrollbar"), //NI
                new ColorAppearanceSetting("Selected Items", "", "MenuHilight"), //NI
                new ColorAppearanceSetting("Tool Tip", "", "InfoWindow"), //I
                new ColorAppearanceSetting("Window", "", "Window"),//I
                new ColorAppearanceSetting("Window Border Width", "BorderWidth", ""),
                new ColorAppearanceSetting("Window Padded Border", "PaddedBorderWidth", "WindowFrame"),
                //new ColorAppearanceSetting("Window Text Color", "", "WindowText") //duplicate to fontsettins
            };
            /*ActiveWindowBorder.Margin = new Thickness((float)(    [29].Size +     [30].Size)); //Window Border Width + Window Padded Border
            InactiveWindowBorder.Margin = ActiveWindowBorder.Margin;*/
        }

        void InitFonts()
        {
            FontSettings = new FontAppearanceSetting[] {
                new FontAppearanceSetting("Active / Inactive Title Font", "CaptionFont", ""),
                new FontAppearanceSetting("Icon Font", "IconFont", ""),
                new FontAppearanceSetting("Menu Font", "MenuFont", "MenuText"), //I
                new FontAppearanceSetting("Palette Title Font", "SmCaptionFont", ""),
                new FontAppearanceSetting("Status Font", "StatusFont", "InfoText"),//I
                new FontAppearanceSetting("Window Text Font", "MessageFont", "WindowText") //I
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

        #region Save
        public async Task SaveChanges()
        {
            if (UseThemes)
            {
                await SaveToTheme();
            }
            else
            {
                await SaveToRegistry();
            }
            await RegistrySettingsViewModel.SaveAll();
            await AeroColorsViewModel.SaveAll();
            KillDWM();
            MessageBox.Show("You need to restart to apply these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        async Task SaveToRegistry()
        {
            List<Task> tasks = new List<Task>();
            foreach (var c in ColorSettings)
            {
                if (c != null) continue;
                c.IsEdited = false;
                tasks.Add(Task.Run(() => c.SaveToRegistry()));
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
            await Task.Delay(2000);
        }
        #endregion

        #region Restore

        void RunRegFile(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "reg.exe";
            startInfo.Arguments = "IMPORT " + path;
            Process.Start(startInfo);
        }

        internal async Task ResetFonts()
        {
            RunRegFile("Window Metrics fix");
            await Task.Delay(100);
        }

        internal async Task ResetColors()
        {
            RunRegFile("Colors fix.reg");
            await Task.Delay(100);
        }

        internal async Task ResetDWM()
        {
            RunRegFile("DWM fix");
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

    }
}
