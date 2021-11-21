using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AdvancedWindowsAppearence
{
    public class GeneralViewModel
    {
        public ColorAppearanceSetting[] ColorSettings { get; set; }
        public FontAppearanceSetting[] FontSettings { get; set; }
        public WallpaperAppearanceSetting Wallpaper { get; set; } = new WallpaperAppearanceSetting();

        public AeroColorRegistrySetting ThemeColor { get; set; } = new AeroColorRegistrySetting("Theme Color", "", "ColorizationColor");
        public RegistrySettingsViewModel RegistrySettingsViewModel { get; set; } = new RegistrySettingsViewModel();
        public AeroColorsViewModel AeroColorsViewModel { get; set; } = new AeroColorsViewModel();
        public double DPI = 1;

        public string ThemeStyle = "";

        public GeneralViewModel()
        {
            InitAeroColors();
            InitRegistrySettings();
            InitColors();
            InitFonts();
            InitDPIScale();
        }

        public void UpdateThemeStyle(string style)
        {
            if (string.IsNullOrEmpty(style))
            {
                ThemeStyle = "";
                return;
            }
            ThemeStyle = @"%SystemRoot%\resources\Themes\" + style + ".msstyles";
        }

        void InitAeroColors()
        {
            AeroColorsViewModel.AddNoCheck("Active Window Color", "AccentColor");
            AeroColorsViewModel.AddNoCheck("Inactive Window Color", "AccentColorInactive");
        }

        void InitDPIScale()
        {
            DPI = System.Windows.SystemFonts.CaptionFontSize / FontSettings[0].FontSize;
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
            ColorSettings = new ColorAppearanceSetting[] {
                new ColorAppearanceSetting("Active Title Color 1", "", "ActiveTitle"),
                new ColorAppearanceSetting("Active Title Color 2", "", "GradientActiveTitle"),
                new ColorAppearanceSetting("Active Title Text", "", "TitleText"),
                new ColorAppearanceSetting("Active Window Border", "", "ActiveBorder"),
                new ColorAppearanceSetting("Application Background", "", "AppWorkspace"),
                new ColorAppearanceSetting("Button Alternate Face", "", "ButtonAlternateFace"),
                new ColorAppearanceSetting("Button Dark Shadow (Right & Bottom)", "", "ButtonDkShadow"),
                new ColorAppearanceSetting("Button Face / 3D Objects", "", "ButtonFace"),
                new ColorAppearanceSetting("Button Light", "", "ButtonLight"),
                new ColorAppearanceSetting("Button Shadow Color", "", "ButtonShadow"),
                new ColorAppearanceSetting("Button Text Color", "", "ButtonText"),
                new ColorAppearanceSetting("Caption Buttons Height", "CaptionHeight", ""),
                new ColorAppearanceSetting("Desktop", "", "Background"),
                new ColorAppearanceSetting("Gray Text", "", "GrayText"),
                new ColorAppearanceSetting("Hilight", "", "Hilight"),
                new ColorAppearanceSetting("Hilighted Text", "", "HilightText"),
                new ColorAppearanceSetting("Hypertext link / Hilight (Fill)", "", "HotTrackingColor"),
                new ColorAppearanceSetting("Icon Size", "Shell Icon Size", ""),
                new ColorAppearanceSetting("Icon Horizontal Spacing", "IconSpacing", ""),
                new ColorAppearanceSetting("Icon Vertical Spacing", "IconVerticalSpacing", ""),
                new ColorAppearanceSetting("Inactive Title Color 1", "", "InactiveTitle"),
                new ColorAppearanceSetting("Inactive Title Color 2", "", "GradientInactiveTitle"),
                new ColorAppearanceSetting("Inactive Title Text", "", "InactiveTitleText"),
                new ColorAppearanceSetting("Inactive Window Border", "", "InactiveBorder"),
                new ColorAppearanceSetting("Menu", "MenuHeight", "Menu"),
                new ColorAppearanceSetting("Scrollbar", "ScrollWidth", "Scrollbar"),
                new ColorAppearanceSetting("Selected Items", "", "MenuHilight"),
                new ColorAppearanceSetting("Tool Tip", "", "InfoWindow"),
                new ColorAppearanceSetting("Window", "", "Window"),
                new ColorAppearanceSetting("Window Border Width", "BorderWidth", ""),
                new ColorAppearanceSetting("Window Padded Border", "PaddedBorderWidth", "WindowFrame"),
                new ColorAppearanceSetting("Window Text Color", "", "WindowText")
            };
            /*ActiveWindowBorder.Margin = new Thickness((float)(    [29].Size +     [30].Size)); //Window Border Width + Window Padded Border
            InactiveWindowBorder.Margin = ActiveWindowBorder.Margin;*/
        }

        void InitFonts()
        {
            FontSettings = new FontAppearanceSetting[] {
                new FontAppearanceSetting("Active / Inactive Title Font", "CaptionFont", ""),
                new FontAppearanceSetting("Icon Font", "IconFont", ""),
                new FontAppearanceSetting("Menu Font", "MenuFont", "MenuText"),
                new FontAppearanceSetting("Palette Title Font", "SmCaptionFont", ""),
                new FontAppearanceSetting("Status Font", "StatusFont", "InfoText"),
                new FontAppearanceSetting("Window Text Font", "MessageFont", "WindowText")
            };
        }

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


        public async Task SaveChanges(string themeName)
        {
            string wallpaperPath = Wallpaper.Path; //finish UI for this



            /* TO DO: FINISH IT
            ThemeSettings SaveTheme = Task.Run(() => new ThemeSettings(themeName, ThemeColor.ItemColor.Value, aeroStyle, wallpaperPath, itemSettings, fontSettings)).Result;

            foreach (var setting in ColorSettings)
            {
                if (setting == null) continue;
                if (!setting.IsEdited) continue;

                if (setting.Font != null)
                    setting.SaveFontToRegistry();
                if (setting.Size.HasValue)
                    setting.SaveSizeToRegistry();

                setting.SaveColorsToRegistry();

                setting.IsEdited = false;
            }

            await Task.Delay(2000);
            await RegistrySettingsViewModel.SaveAll();
            AeroColorsViewModel.SaveAll();*/
            KillDWM();
            MessageBox.Show("You need to restart to apply these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        void RunRegFile(string path) {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "reg.exe";
            startInfo.Arguments = "IMPORT " + path;
            Process.Start(startInfo);
        }

        public async Task ResetToDefaults()
        {
            RunRegFile("Colors fix.reg");
            RunRegFile("Window Metrics fix");
            await Task.Delay(200);
        }
    }
}
