using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class GeneralViewModel
    {
        public AppearanceSetting[] ColorSettings = new ColorAppearanceSetting[32];
        public AppearanceSetting[] FontSettings = new FontAppearanceSetting[6];
        public WallpaperAppearanceSetting Wallpaper = new WallpaperAppearanceSetting();

        void LoadColors()
        {
            ColorSettings[0] = new ColorAppearanceSetting("Active Title Color 1", "", "ActiveTitle");
            ColorSettings[1] = new ColorAppearanceSetting("Active Title Color 2", "", "GradientActiveTitle");
            ColorSettings[2] = new ColorAppearanceSetting("Active Title Text", "", "TitleText");
            ColorSettings[3] = new ColorAppearanceSetting("Active Window Border", "", "ActiveBorder");
            ColorSettings[4] = new ColorAppearanceSetting("Application Background", "", "AppWorkspace");
            ColorSettings[5] = new ColorAppearanceSetting("Button Alternate Face", "", "ButtonAlternateFace");
            ColorSettings[6] = new ColorAppearanceSetting("Button Dark Shadow (Right & Bottom)", "", "ButtonDkShadow");
            ColorSettings[7] = new ColorAppearanceSetting("Button Face / 3D Objects", "", "ButtonFace");
            ColorSettings[8] = new ColorAppearanceSetting("Button Light", "", "ButtonLight");
            ColorSettings[9] = new ColorAppearanceSetting("Button Shadow Color", "", "ButtonShadow");
            ColorSettings[10] = new ColorAppearanceSetting("Button Text Color", "", "ButtonText");
            ColorSettings[11] = new ColorAppearanceSetting("Caption Buttons Height", "CaptionHeight", "");
            ColorSettings[12] = new ColorAppearanceSetting("Desktop", "", "Background");
            ColorSettings[13] = new ColorAppearanceSetting("Gray Text", "", "GrayText");
            ColorSettings[14] = new ColorAppearanceSetting("Hilight", "", "Hilight");
            ColorSettings[15] = new ColorAppearanceSetting("Hilighted Text", "", "HilightText");
            ColorSettings[16] = new ColorAppearanceSetting("Hypertext link / Hilight (Fill)", "", "HotTrackingColor");
            ColorSettings[17] = new ColorAppearanceSetting("Icon Size", "Shell Icon Size", "");
            ColorSettings[18] = new ColorAppearanceSetting("Icon Horizontal Spacing", "IconSpacing", "");
            ColorSettings[19] = new ColorAppearanceSetting("Icon Vertical Spacing", "IconVerticalSpacing", "");
            ColorSettings[20] = new ColorAppearanceSetting("Inactive Title Color 1", "", "InactiveTitle");
            ColorSettings[21] = new ColorAppearanceSetting("Inactive Title Color 2", "", "GradientInactiveTitle");
            ColorSettings[22] = new ColorAppearanceSetting("Inactive Title Text", "", "InactiveTitleText");
            ColorSettings[23] = new ColorAppearanceSetting("Inactive Window Border", "", "InactiveBorder");
            ColorSettings[24] = new ColorAppearanceSetting("Menu", "MenuHeight", "Menu");
            ColorSettings[25] = new ColorAppearanceSetting("Scrollbar", "ScrollWidth", "Scrollbar");
            ColorSettings[26] = new ColorAppearanceSetting("Selected Items", "", "MenuHilight");
            ColorSettings[27] = new ColorAppearanceSetting("Tool Tip", "", "InfoWindow");
            ColorSettings[28] = new ColorAppearanceSetting("Window", "", "Window");
            ColorSettings[29] = new ColorAppearanceSetting("Window Border Width", "BorderWidth", "");
            ColorSettings[30] = new ColorAppearanceSetting("Window Padded Border", "PaddedBorderWidth", "WindowFrame");
            ColorSettings[31] = new ColorAppearanceSetting("Window Text Color", "", "WindowText");
            /*ActiveWindowBorder.Margin = new Thickness((float)(ColorSettings[29].Size + ColorSettings[30].Size)); //Window Border Width + Window Padded Border
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

    }
}
