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
        public ColorAppearanceSetting[] ColorSettings = new ColorAppearanceSetting[32];
        public FontAppearanceSetting[] FontSettings = new FontAppearanceSetting[6];
        public WallpaperAppearanceSetting Wallpaper = new WallpaperAppearanceSetting();

        public AeroColorRegistrySetting ThemeColor = new AeroColorRegistrySetting("Theme Color", "", "ColorizationColor");
        public RegistrySettingsViewModel RegistrySettingsViewModel = new RegistrySettingsViewModel();
        public AeroColorsViewModel AeroColorsViewModel = new AeroColorsViewModel();
        public double DPI = 1;

        public string ThemeStyle = "";

        public void UpdateThemeStyle(string style)
        {
            if (string.IsNullOrEmpty(style))
            {
                ThemeStyle = "";
                return;
            }
            ThemeStyle = @"%SystemRoot%\resources\Themes\" + style + ".msstyles";
        }

        void LoadDPIScale()
        {
            DPI = SystemFonts.CaptionFont.Size / FontSettings[0].FontSize;
        }

        void LoadColors()
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

        void LoadFonts()
        {
            FontSettings[0] = new FontAppearanceSetting("Active / Inactive Title Font", "CaptionFont", "");
            FontSettings[1] = new FontAppearanceSetting("Icon Font", "IconFont", "");
            FontSettings[2] = new FontAppearanceSetting("Menu Font", "MenuFont", "MenuText");
            FontSettings[3] = new FontAppearanceSetting("Palette Title Font", "SmCaptionFont", "");
            FontSettings[4] = new FontAppearanceSetting("Status Font", "StatusFont", "InfoText");
            FontSettings[5] = new FontAppearanceSetting("Window Text Font", "MessageFont", "WindowText");
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

    }
}
