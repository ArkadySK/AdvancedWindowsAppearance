using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AppearanceSetting[] appearanceSettings = new AppearanceSetting[21];
        AppearanceSetting AeroColors = new AppearanceSetting("Aero Colors", "ColorizationColor", "ColorizationColorInactive");
        bool isHighContrast = false; //dokonci
        
        public MainWindow()
        {
            InitializeComponent();
            LoadItems();
            LoadAeroTab();
            UpdateFontList();
            UpdateWallpaperInfo();
            LoadThemeName();
        }
        Brush MediaColorToBrush(System.Drawing.Color? col)
        {
            if (col == null)
                return null;
            
            Brush returnbrush = new SolidColorBrush(Color.FromArgb(col.Value.A, col.Value.R, col.Value.G, col.Value.B));
            return returnbrush;
        }

        void LoadAeroTab()
        {
            if (!AeroColors.Color1.HasValue)
                return;
            byte a = AeroColors.Color1.Value.A;
            imageAeroColor.Background = MediaColorToBrush(AeroColors.Color1);
            textBoxColorOpacity.Text = a.ToString();
            sliderColorOpacity.Value = a;
        }

        void LoadThemeName()
        {
            ThemeSettings themeSettings = new ThemeSettings();
            string themeName = themeSettings.GetThemePath();

            themeName = themeName.Split("\\".ToCharArray()).Last();
            themeName = themeName.Replace(".theme", "");

            if (!themeName.Contains(" (Edited)"))
                textBoxThemeName.Text = themeName + " (Edited)";
            else
                textBoxThemeName.Text = themeName;

        }

        void LoadItems()
        {
            
            comboBoxItems.Items.Clear();

            // chyba: ButtonShadow, ButtonDkShadow
            appearanceSettings[0] = new AppearanceSetting("3D Objects", "", "", "WindowText", "ButtonFace", "ButtonLight");
            appearanceSettings[1] = new AppearanceSetting("Active Title Bar", "CaptionHeight", "CaptionFont", "TitleText", "ActiveTitle", "GradientActiveTitle");
            appearanceSettings[2] = new AppearanceSetting("Active Window Border", "BorderWidth", "", "", "ActiveBorder", "");
            appearanceSettings[3] = new AppearanceSetting("Application Background", "", "", "", "AppWorkspace", "ButtonAlternateFace");
            appearanceSettings[4] = new AppearanceSetting("Caption Buttons", "CaptionHeight", "", "", "", "");
            appearanceSettings[5] = new AppearanceSetting("Desktop", "", "", "", "Background", "");
            appearanceSettings[6] = new AppearanceSetting("Hypertext link", "", "", "", "HotTrackingColor", "Hilight");
            appearanceSettings[7] = new AppearanceSetting("Icon", "Shell Icon Size", "IconFont", "", "", "");
            appearanceSettings[8] = new AppearanceSetting("Icon Horizontal Spacing", "IconSpacing", "", "", "", "");
            appearanceSettings[9] = new AppearanceSetting("Icon Vertical Spacing", "IconVerticalSpacing", "", "", "", "");
            appearanceSettings[10] = new AppearanceSetting("Inactive Title Bar", "CaptionHeight", "CaptionFont", "InactiveTitleText", "InactiveTitle", "GradientInactiveTitle");
            appearanceSettings[11] = new AppearanceSetting("Inactive Window Border", "BorderWidth", "", "", "InactiveBorder", "");
            appearanceSettings[12] = new AppearanceSetting("Menu", "MenuHeight", "MenuFont", "MenuText", "Menu", "MenuBar");
            appearanceSettings[13] = new AppearanceSetting("Message Box", "", "MessageFont", "WindowText", "", "");
            appearanceSettings[14] = new AppearanceSetting("Palette Title", "", "MessageFont", "WindowText", "", ""); //co to je??
            appearanceSettings[15] = new AppearanceSetting("Scrollbar", "ScrollWidth", "", "", "Scrollbar", "");
            appearanceSettings[16] = new AppearanceSetting("Selected Items", "ScrollWidth", "MenuFont", "HilightText", "MenuHilight", "");
            appearanceSettings[17] = new AppearanceSetting("Tool Tip", "", "StatusFont", "InfoText", "InfoWindow", "");
            appearanceSettings[18] = new AppearanceSetting("Window", "", "", "WindowText", "Window", "");
            appearanceSettings[19] = new AppearanceSetting("Window Padded Border", "PaddedBorderWidth", "", "", "WindowFrame", "");
           



            //appearanceSettings[8] = new AppearanceSetting(


            foreach (var a in appearanceSettings)
            {
                if (a != null)
                    comboBoxItems.Items.Add(a.Name);
            }

            SelectSetting(appearanceSettings[5]);
        }

        List<System.Drawing.Font> GetSystemFonts()
        {
            List<System.Drawing.Font> fonts = new List<System.Drawing.Font>();

            foreach (System.Drawing.FontFamily font in System.Drawing.FontFamily.Families)
            {
                System.Drawing.Font f = new System.Drawing.Font(font, 11);
                fonts.Add(f);

            }
            return fonts;
        }

        void UpdateFontList()
        {
            comboBoxFonts.Items.Clear();
            List<System.Drawing.Font> fonts = GetSystemFonts();
            foreach (var f in fonts)
            {
                comboBoxFonts.Items.Add(f.Name);
            }
        }

        void UpdateWallpaperInfo()
        {
            ImageWallpaper.Source = GetCurrentWallpaperSource();
            ImageWallpaper.Stretch = Stretch.UniformToFill;
        }

        ImageSource GetCurrentWallpaperSource()
        {
            ImageSource wallpaper;
            AppearanceSetting appearanceSettingWallpaper = new AppearanceSetting("Wallpaper", "");
            string path = appearanceSettingWallpaper.GetWallpaperPath();
            if (path == "") return null;
            wallpaper = new BitmapImage(new Uri(path));
            return wallpaper;
        }

        AppearanceSetting GetSelSetting()
        {
            return appearanceSettings[comboBoxItems.SelectedIndex]; 
        }

        void UpdateItemInfo(float? nullableFloat)
        {
            textBoxItemSize.IsEnabled = nullableFloat.HasValue;

            if (nullableFloat == null) {
                textBoxItemSize.Text = "";           
            }
            else
            {
                textBoxItemSize.Text = nullableFloat.Value.ToString();
            }
        }

        void UpdateFontInfo(AppearanceSetting selSetting)
        {
            if (selSetting.Font == null)
            {
                comboBoxFonts.IsEnabled = false;
                comboBoxFonts.Text = "";
                comboBoxFontSize.Text = "";
                buttonFontBold.IsEnabled = false;
                buttonFontItalic.IsEnabled = false;
                comboBoxFontSize.IsEnabled = false;
            }
            else
            {
                //comboBoxFonts.IsEnabled = true;
                comboBoxFonts.Text = selSetting.Font.Name;
                comboBoxFontSize.Text = selSetting.Font.Size.ToString();
                buttonFontBold.IsEnabled = true;
                buttonFontItalic.IsEnabled = true;
                buttonFontBold.IsChecked = selSetting.Font_isBold;
                buttonFontItalic.IsChecked = selSetting.Font_isItalic;              
                //comboBoxFontSize.IsEnabled = true;
            }
        }

        void UpdateColors(AppearanceSetting appearanceSetting)
        {
            if (appearanceSetting.Color1.HasValue)
            {
                imageItemColor1.Background = MediaColorToBrush(appearanceSetting.Color1);
                buttonItemColor1.IsEnabled = true;
            }
            else
                buttonItemColor1.IsEnabled = false;

            if (appearanceSetting.Color2.HasValue)
            {
                imageItemColor2.Background = MediaColorToBrush(appearanceSetting.Color2);
                buttonItemColor2.IsEnabled = true;
            }
            else
                buttonItemColor2.IsEnabled = false;

            if (appearanceSetting.FontColor.HasValue)
            {
                imageFontColor.Background = MediaColorToBrush(appearanceSetting.FontColor);
                buttonFontColor.IsEnabled = true;
            }
            else
                buttonFontColor.IsEnabled = false;

        }

        void SelectSetting(AppearanceSetting appearanceSetting)
        {
            UpdateColors(appearanceSetting);
            UpdateItemInfo(appearanceSetting.Size);
            UpdateFontInfo(appearanceSetting);
            comboBoxItems.Text = appearanceSetting.Name;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) ///apply button
        {
           
        }

        private void comboBoxItems_DropDownClosed(object sender, EventArgs e)
        {
            labelComboBoxItems.Content = comboBoxItems.Text;
            int i = comboBoxItems.SelectedIndex;
            if(i!=-1)
            SelectSetting(appearanceSettings[i]);
        }

        private void comboBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            labelComboBoxItems.Content = comboBoxItems.Text;
            int i = comboBoxItems.SelectedIndex;
            if (i != -1)
            SelectSetting(appearanceSettings[i]);
        }

        private void labelComboBoxItems_MouseDown(object sender, MouseButtonEventArgs e)
        {
            comboBoxItems.IsDropDownOpen = true;
        }

        System.Drawing.Color OpenColorDialog(System.Drawing.Color? defaultCol)
        {
            System.Drawing.Color color = defaultCol.Value;
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            //colorDialog.AnyColor = true;
            colorDialog.Color = color;
            colorDialog.FullOpen = true;
            System.Windows.Forms.DialogResult dialogResult = colorDialog.ShowDialog();
            if(dialogResult== System.Windows.Forms.DialogResult.OK)
            {
                color = System.Drawing.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            }
            return color;
        }

        private void buttonEditItemColor1_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelSetting();
            selSetting.Color1 = OpenColorDialog(selSetting.Color1);
            UpdateColors(selSetting);
            selSetting.IsEdited = true;
        }

        private void buttonEditItemColor2_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelSetting();
            selSetting.Color2 = OpenColorDialog(selSetting.Color2);
            UpdateColors(selSetting);
            selSetting.IsEdited = true;
        }
        private void buttonEditFontColor_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelSetting();
            selSetting.FontColor = OpenColorDialog(selSetting.FontColor);
            UpdateColors(selSetting);
            selSetting.IsEdited = true;
        }

        private void buttonFontBold_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = appearanceSettings[comboBoxItems.SelectedIndex];
            selSetting.ChangeFontBoldness(buttonFontBold.IsChecked.Value);
        }

        private void buttonFontItalic_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelSetting();
            selSetting.ChangeFontItalicness(buttonFontItalic.IsChecked.Value);
        }

        private void textBoxItemSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxItemSize.Text == "" || comboBoxItems.SelectedIndex == -1) return;

            var selSetting = GetSelSetting();
            selSetting.ChangeSize(int.Parse(textBoxItemSize.Text, System.Globalization.NumberStyles.Integer));
        }

        #region Save Changes

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        string aeroStyle = "";

        void SaveChanges() { // fixni fonty
            //@"%SystemRoot%\resources\Themes\Aero\Aero.msstyles"; //fixni UI nech sa to da zvolit
            if(!checkBoxOverwriteThemeStyle.IsChecked.Value)
            {
                isHighContrast = false;
                aeroStyle = "";
            }
            string themeName = textBoxThemeName.Text;

            string wallpaperPath = ""; //chyyba na to UI 
            foreach(var s in appearanceSettings)
            {
                if(s!=null && s.IsEdited)
                s.ConvertColorValuesToRegedit();
            }
            ThemeSettings SaveTheme = new ThemeSettings(themeName, AeroColors.Color1.Value, aeroStyle, wallpaperPath, appearanceSettings);
            
            foreach (var setting in appearanceSettings)
            {
                if (setting == null) continue;
                if (!setting.IsEdited) continue;

                if (setting.Font!=null)
                setting.SaveFontToRegedit();
                if (setting.Size.HasValue)
                setting.SaveSizeToRegedit();

                setting.SaveColorsToRegedit();

                setting.IsEdited = false;
            }
            MessageBox.Show("You need to log off or restart to apply these changes.", "Logoff required", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region Theme Style
        void UpdateThemeStyle(string style)
        {
            if (style == "") aeroStyle = "";
            aeroStyle = @"%SystemRoot%\resources\Themes\"+style+ ".msstyles";
        }

        private void ToggleButtonAero_Click(object sender, RoutedEventArgs e)
        {
            UpdateThemeStyle("Aero\\Aero");
            isHighContrast = false;
        }

        private void ToggleButtonAeroLite_Click(object sender, RoutedEventArgs e)
        {
            UpdateThemeStyle("Aero\\AeroLite");
            isHighContrast = false;
        }

        private void ToggleButtonHighContrast_Click(object sender, RoutedEventArgs e)
        {
            UpdateThemeStyle("Aero\\AeroLite");
            isHighContrast = true;
        }

        private void CheckBoxOverwriteThemeStyle_Click(object sender, RoutedEventArgs e)
        {
            
            stackPanelAeroSettingsButtons.IsEnabled = checkBoxOverwriteThemeStyle.IsChecked.Value;
        }

        private void checkBoxOverwriteThemeStyle_Checked(object sender, RoutedEventArgs e)
        {
            if (!checkBoxOverwriteThemeStyle.IsChecked.Value)
            {
                checkBoxOverwriteThemeStyle.IsChecked = false;
                UpdateThemeStyle("");
            }
            else if (checkBoxOverwriteThemeStyle.IsChecked.Value)
                checkBoxOverwriteThemeStyle.IsChecked = true;

        }
        #endregion


        #region Aero Tab

        private void buttonAeroColor_Click(object sender, RoutedEventArgs e)
        {
            AeroColors.Color1 = OpenColorDialog(AeroColors.Color2);
            LoadAeroTab();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void textBoxColorOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            string opacityText = textBoxColorOpacity.Text;
            if (opacityText == null || opacityText=="")
                return;
            int alpha = int.Parse(opacityText);
            byte a = new byte();
            if (alpha > byte.MaxValue || alpha < byte.MinValue) return;
            a = byte.Parse(alpha.ToString());
            AeroColors.Color1 = System.Drawing.Color.FromArgb(alpha ,AeroColors.Color1.Value.R, AeroColors.Color1.Value.G, AeroColors.Color1.Value.B);
            LoadAeroTab();
        }

        private void sliderColorOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte a = new byte();
            a = Convert.ToByte(sliderColorOpacity.Value);
            AeroColors.Color1 = System.Drawing.Color.FromArgb(a, AeroColors.Color1.Value.R, AeroColors.Color1.Value.G, AeroColors.Color1.Value.B);
            LoadAeroTab();
        }
        #endregion

    }
}
