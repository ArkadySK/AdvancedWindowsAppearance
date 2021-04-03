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

        AppearanceSetting[] itemSettings = new AppearanceSetting[32];
        AppearanceSetting[] fontSettings = new AppearanceSetting[6];
        AeroColorRegistrySetting themeColor = new AeroColorRegistrySetting("Aero Colors", "", "ColorizationColor");

        RegistrySettingsControler RegistrySettingsController = new RegistrySettingsControler();
        AeroColorsController AeroColorsController = new AeroColorsController();


        bool isHighContrast = false; //finish this feature
        
        public MainWindow()
        {
            InitializeComponent();
            LoadItems();
            UpdateFontList();
            LoadFonts();
            LoadDPIScale();
            LoadAeroTab();
            LoadAeroColors();
            UpdateWallpaperInfo();
            LoadThemeName();
            LoadRegistrySettings();

            SelectItem(itemSettings[12]);
        }

        void LoadAeroTab()
        {
            if (!themeColor.ItemColor.HasValue)
                return;
            byte a = themeColor.ItemColor.Value.A;
            imageThemeColor.Background = MediaColorToBrush(themeColor.ItemColor);
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

        double dpi = 1;

        void LoadDPIScale()
        {
            dpi = SystemFonts.CaptionFontSize / fontSettings[0].FontSize;
        }

        void LoadItems()
        {
            
            comboBoxItems.Items.Clear();

            itemSettings[0] = new AppearanceSetting("Active Title Color 1", "", "ActiveTitle", "Item");
            itemSettings[1] = new AppearanceSetting("Active Title Color 2", "", "GradientActiveTitle", "Item");
            itemSettings[2] = new AppearanceSetting("Active Title Text", "", "TitleText", "Item");
            itemSettings[3] = new AppearanceSetting("Active Window Border", "", "ActiveBorder", "Item");
            itemSettings[4] = new AppearanceSetting("Application Background", "", "AppWorkspace", "Item");
            itemSettings[5] = new AppearanceSetting("Button Alternate Face", "", "ButtonAlternateFace", "Item");
            itemSettings[6] = new AppearanceSetting("Button Dark Shadow (Right & Bottom)", "", "ButtonDkShadow", "Item");
            itemSettings[7] = new AppearanceSetting("Button Face / 3D Objects", "", "ButtonFace", "Item");
            itemSettings[8] = new AppearanceSetting("Button Light", "", "ButtonLight", "Item");
            itemSettings[9] = new AppearanceSetting("Button Shadow Color", "", "ButtonShadow", "Item");
            itemSettings[10] = new AppearanceSetting("Button Text Color", "", "ButtonText", "Item");
            itemSettings[11] = new AppearanceSetting("Caption Buttons Height", "CaptionHeight", "", "Item");
            itemSettings[12] = new AppearanceSetting("Desktop", "", "Background", "Item");
            itemSettings[13] = new AppearanceSetting("Gray Text", "", "GrayText", "Item");
            itemSettings[14] = new AppearanceSetting("Hilight", "", "Hilight", "Item");
            itemSettings[15] = new AppearanceSetting("Hilighted Text", "", "HilightText", "Item");
            itemSettings[16] = new AppearanceSetting("Hypertext link / Hilight (Fill)", "", "HotTrackingColor", "Item");
            itemSettings[17] = new AppearanceSetting("Icon Size", "Shell Icon Size", "", "Item");
            itemSettings[18] = new AppearanceSetting("Icon Horizontal Spacing", "IconSpacing", "", "Item");
            itemSettings[19] = new AppearanceSetting("Icon Vertical Spacing", "IconVerticalSpacing", "", "Item");
            itemSettings[20] = new AppearanceSetting("Inactive Title Color 1", "", "InactiveTitle", "Item");
            itemSettings[21] = new AppearanceSetting("Inactive Title Color 2", "", "GradientInactiveTitle", "Item");
            itemSettings[22] = new AppearanceSetting("Inactive Title Text", "", "InactiveTitleText", "Item");
            itemSettings[23] = new AppearanceSetting("Inactive Window Border", "", "InactiveBorder", "Item");
            itemSettings[24] = new AppearanceSetting("Menu", "MenuHeight", "Menu", "Item");
            itemSettings[25] = new AppearanceSetting("Scrollbar", "ScrollWidth", "Scrollbar", "Item");
            itemSettings[26] = new AppearanceSetting("Selected Items", "", "MenuHilight", "Item");
            itemSettings[27] = new AppearanceSetting("Tool Tip", "", "InfoWindow", "Item");
            itemSettings[28] = new AppearanceSetting("Window", "", "Window", "Item");
            itemSettings[29] = new AppearanceSetting("Window Border Width", "BorderWidth", "", "Item");
            itemSettings[30] = new AppearanceSetting("Window Padded Border", "PaddedBorderWidth", "WindowFrame", "Item");
            itemSettings[31] = new AppearanceSetting("Window Text Color", "", "WindowText", "Item");

            ActiveWindowBorder.Margin = new Thickness((float)(itemSettings[29].Size + itemSettings[30].Size)); //Window Border Width + Window Padded Border
            InactiveWindowBorder.Margin = ActiveWindowBorder.Margin;

            foreach (var s in itemSettings)
            {               
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                if (s == null)
                    comboBoxItem.Visibility = Visibility.Collapsed;
                else
                {
                    comboBoxItem.Content = s.Name;
                    if (s.ItemColor == null && s.Size == null) { 
                        comboBoxItem.Visibility = Visibility.Collapsed;
                        comboBoxItem.Height = 0;
                    }
                }              
                comboBoxItems.Items.Add(comboBoxItem);                            
            }
        }

        void LoadFonts()
        {
            fontSettings[0] = new AppearanceSetting("Active / Inactive Title Font", "CaptionFont", "", "Font");
            fontSettings[1] = new AppearanceSetting("Icon Font", "IconFont", "", "Font");
            fontSettings[2] = new AppearanceSetting("Menu Font", "MenuFont", "MenuText", "Font");
            fontSettings[3] = new AppearanceSetting("Palette Title Font", "SmCaptionFont", "", "Font");
            fontSettings[4] = new AppearanceSetting("Status Font", "StatusFont", "InfoText", "Font");
            fontSettings[5] = new AppearanceSetting("Window Text Font", "MessageFont", "WindowText", "Font");

            foreach (var a in fontSettings)
            {
                if (a != null)
                    comboBoxFonts.Items.Add(a.Name);
            }

            SelectFont(fontSettings[0]);
        }

        void LoadAeroColors()
        {
            comboBoxAeroColors.DataContext = AeroColorsController;
            AeroColorsController.Add("Active Window Color", "AccentColor");
            AeroColorsController.Add("Active Window Border", "ColorizationColor");
            AeroColorsController.Add("Inactive Window Color", "AccentColorInactive");
        }

        
        void LoadRegistrySettings()
        {
            listBoxDMW.DataContext = RegistrySettingsController;
            RegistrySettingsController.Add("Show accent color on the title bars", "ColorPrevalence", new Version(10,0));
            RegistrySettingsController.Add("Enable transparent taskbar", "ColorizationOpaqueBlend", new Version(6, 2));
            //RegistrySettingsController.Add("Enable composition", "Composition", new Version(6, 1));           
            RegistrySettingsController.Add("Enable peek at desktop", "EnableAeroPeek", new Version(6, 1));
            RegistrySettingsController.Add("Hibernate Thumbnails", "AlwaysHibernateThumbnails", new Version(6, 1));
            RegistrySettingsController.AddWithPath("Enable transparency effects", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", new Version(10, 0));
            RegistrySettingsController.AddWithPath("Show accent color on the start and actioncenter", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", new Version(10,0));
            RegistrySettingsController.AddWithPath("Apps use light theme", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", new Version(10, 0));
            RegistrySettingsController.AddWithPath("System uses light theme", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", new Version(10, 0));
            RegistrySettingsController.AddWithPath("Always show scrollbars in modern apps", @"Control Panel\Accessibility", "DynamicScrollbars", new Version(10,0));
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
            comboBoxFont.Items.Clear();
            List<System.Drawing.Font> fonts = GetSystemFonts();
            foreach (var f in fonts)
            {
                comboBoxFont.Items.Add(f.Name);
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

        AppearanceSetting GetSelItemSetting()
        {
            return itemSettings[comboBoxItems.SelectedIndex]; 
        }

        AppearanceSetting GetSelFontSetting()
        {
            return fontSettings[comboBoxFonts.SelectedIndex];
        }

        void UpdateItemSize(float? nullableFloat)
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
                comboBoxFont.IsEnabled = false;
                comboBoxFont.Text = "";
                comboBoxFontSize.Text = "";
                buttonFontBold.IsEnabled = false;
                buttonFontItalic.IsEnabled = false;
                comboBoxFontSize.IsEnabled = false;
                textBlockPreview.Content = null;
            }
            else
            {
                comboBoxFont.IsEnabled = true;
                comboBoxFont.Text = selSetting.Font.Name;
                comboBoxFontSize.Text = selSetting.FontSize.ToString();
                buttonFontBold.IsEnabled = true;
                buttonFontItalic.IsEnabled = true;
                buttonFontBold.IsChecked = selSetting.Font_isBold;
                buttonFontItalic.IsChecked = selSetting.Font_isItalic;              
                comboBoxFontSize.IsEnabled = true;
                UpdateFontPreview(selSetting.Font, selSetting.FontSize, selSetting.Font_isBold, selSetting.Font_isItalic, selSetting.FontColor);
            }

        }

        void UpdateFontPreview(System.Drawing.Font f, int size, bool isBold, bool isItalic, System.Drawing.Color? textCol)
        {
            var textColtemp = System.Drawing.Color.Black;
            if (textCol.HasValue)
                textColtemp = textCol.Value;
            UpdateFontPreview(f, size);
            UpdateFontPreview(isBold, isItalic);
            UpdateFontPreview(textColtemp);
        }

        void UpdateFontPreview(bool isBold, bool isItalic)
        {
            if (isItalic)
                textBlockPreview.FontStyle = FontStyles.Italic;
            else
                textBlockPreview.FontStyle = FontStyles.Normal;
            if (isBold)
                textBlockPreview.FontWeight = FontWeights.Bold;
            else
                textBlockPreview.FontWeight = FontWeights.Normal;           
        }

        void UpdateFontPreview(System.Drawing.Color? textCol)
        {
            textBlockPreview.Foreground = MediaColorToBrush(textCol);
        }

        void UpdateFontPreview(System.Drawing.Font f, int size)
        {
            textBlockPreview.Content = "The quick brown fox jumps over the lazy dog";
            textBlockPreview.FontFamily = new System.Windows.Media.FontFamily(f.FontFamily.Name);
            textBlockPreview.FontSize = size * dpi;
        }

        void UpdateColors(AppearanceSetting appearanceSetting)
        {
            if (appearanceSetting.ItemColor.HasValue)
            {
                imageItemColor.Background = MediaColorToBrush(appearanceSetting.ItemColor);
                buttonItemColor.IsEnabled = true;
            }
            else
                buttonItemColor.IsEnabled = false;

            
            if (appearanceSetting.FontColor.HasValue)
            {
                imageFontColor.Background = MediaColorToBrush(appearanceSetting.FontColor);
                buttonFontColor.IsEnabled = true;
            }
            else
                buttonFontColor.IsEnabled = false;

        }

        void SelectItem(AppearanceSetting appearanceSetting)
        {
            UpdateColors(appearanceSetting);
            UpdateItemSize(appearanceSetting.Size);
            comboBoxItems.Text = appearanceSetting.Name;
        }

        void SelectFont(AppearanceSetting appearanceSetting)
        {
            UpdateColors(appearanceSetting);
            UpdateFontInfo(appearanceSetting);
            comboBoxFonts.Text = appearanceSetting.Name;
        }

        Brush MediaColorToBrush(System.Drawing.Color? col)
        {
            if (col == null)
                return null;

            Brush returnbrush = new SolidColorBrush(Color.FromArgb(col.Value.A, col.Value.R, col.Value.G, col.Value.B));
            return returnbrush;
        }

        System.Drawing.Color OpenColorDialog(System.Drawing.Color? defaultCol)
        {
            System.Drawing.Color color = new System.Drawing.Color();
            if (defaultCol.HasValue)
                color = defaultCol.Value;
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            //colorDialog.AnyColor = true;
            colorDialog.Color = color;
            colorDialog.FullOpen = true;
            System.Windows.Forms.DialogResult dialogResult = colorDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                color = System.Drawing.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
            }
            return color;
        }


        #region Colors And Metrics Tab
        private void comboBoxItems_DropDownClosed(object sender, EventArgs e)
        {
            labelComboBoxItems.Content = comboBoxItems.Text;
            int i = comboBoxItems.SelectedIndex;
            if(i!=-1)
                SelectItem(itemSettings[i]);
        }

        private void comboBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            labelComboBoxItems.Content = comboBoxItems.Text;
            int i = comboBoxItems.SelectedIndex;
            if (i != -1)
                SelectItem(itemSettings[i]);
        }

        private void labelComboBoxItems_MouseDown(object sender, MouseButtonEventArgs e) //
        {
            comboBoxItems.IsDropDownOpen = true;
        }

        private void buttonEditItemColor_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelItemSetting();
            selSetting.ItemColor = OpenColorDialog(selSetting.ItemColor);
            UpdateColors(selSetting);
            selSetting.IsEdited = true;
        }

        private void textBoxItemSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxItemSize.Text == "" || comboBoxItems.SelectedIndex == -1) return;

            var selSetting = GetSelItemSetting();
            selSetting.ChangeSize(int.Parse(textBoxItemSize.Text, System.Globalization.NumberStyles.Integer));
        }
        #endregion


        #region Fonts Tab
        private void comboBoxFonts_DropDownClosed(object sender, EventArgs e)
        {
            int i = comboBoxFonts.SelectedIndex;
            if (i != -1)
                SelectFont(fontSettings[i]);
        }

        private void comboBoxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = comboBoxFonts.SelectedIndex;
            if (i != -1)
                SelectFont(fontSettings[i]);
        }

        private void buttonEditFontColor_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelFontSetting();
            selSetting.FontColor = OpenColorDialog(selSetting.FontColor);
            UpdateColors(selSetting);
            selSetting.IsEdited = true;
            UpdateFontPreview(selSetting.FontColor.Value);
        }

        private void buttonFontBold_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelFontSetting();
            selSetting.ChangeFontBoldness(buttonFontBold.IsChecked.Value);
            UpdateFontPreview(selSetting.Font_isBold, selSetting.Font_isItalic);
        }

        private void buttonFontItalic_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelFontSetting();
            selSetting.ChangeFontItalicness(buttonFontItalic.IsChecked.Value);
            UpdateFontPreview(selSetting.Font_isBold, selSetting.Font_isItalic);
        }

        private void comboBoxFont_TextInput(object sender, TextCompositionEventArgs e)
        {
            /*if (comboBoxFonts.SelectedIndex == -1) return;
            var selSetting = GetSelFontSetting();
            selSetting.ChangeFontName(comboBoxFont.Text);
            Console.WriteLine(comboBoxFont.Text);*/
        }

        private void comboBoxFont_DropDownClosed(object sender, EventArgs e)
        {
            if(comboBoxFonts.SelectedIndex == -1) return;
            var selSetting = GetSelFontSetting();
            selSetting.ChangeFontName(comboBoxFont.Text);          
            UpdateFontPreview(selSetting.Font, selSetting.FontSize);
        }

        private void comboBoxFontSize_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxFonts.SelectedIndex == -1) return;
            var s = GetSelFontSetting();
            int size = int.Parse(comboBoxFontSize.Text);
            s.FontSize = size;
            UpdateFontPreview(s.Font, size);
        }

        #endregion


        #region Save Changes

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveChanges();
        }

        string aeroStyle = "";

        void SaveChanges() {         
            if (!checkBoxOverwriteThemeStyle.IsChecked.Value)
            {
                isHighContrast = false;
                aeroStyle = "";
            }

            string themeName = textBoxThemeName.Text;
            string wallpaperPath = ""; //chyyba na to UI 

            
            ThemeSettings SaveTheme = new ThemeSettings(themeName, themeColor.ItemColor.Value, aeroStyle, wallpaperPath, itemSettings, fontSettings);
            
            foreach (var setting in itemSettings)
            {
                if (setting == null) continue;
                if (!setting.IsEdited) continue;

                if (setting.Font!=null)
                setting.SaveFontToRegistry();
                if (setting.Size.HasValue)
                setting.SaveSizeToRegistry();

                setting.SaveColorsToRegistry();

                setting.IsEdited = false;
            }
            MessageBox.Show("You need to restart to apply these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);

            AeroColorsController.SaveAll();
            RegistrySettingsController.SaveAll();
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

        private void buttonThemeColor_Click(object sender, RoutedEventArgs e)
        {
            themeColor.ItemColor = OpenColorDialog(themeColor.ItemColor);
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
            themeColor.ItemColor = System.Drawing.Color.FromArgb(alpha ,themeColor.ItemColor.Value.R, themeColor.ItemColor.Value.G, themeColor.ItemColor.Value.B);
            LoadAeroTab();
        }

        private void sliderColorOpacity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            byte a = new byte();
            a = Convert.ToByte(sliderColorOpacity.Value);
            themeColor.ItemColor = System.Drawing.Color.FromArgb(a, themeColor.ItemColor.Value.R, themeColor.ItemColor.Value.G, themeColor.ItemColor.Value.B);
            LoadAeroTab();
        }
        #endregion


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        int f_count = 0; //extra kek
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
                f_count++;
            if(f_count >= 10)
            {
                pageModern.Visibility = Visibility.Visible;
            }
        }

        private void buttonAeroColor_Click(object sender, RoutedEventArgs e)
        {
            AeroColorsController.ChangeColorCurrent((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem);
            OpenColorDialog(((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem).ItemColor);
            LoadAeroTab();
        }
    }
}
