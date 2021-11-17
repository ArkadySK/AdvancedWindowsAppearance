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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GeneralViewModel Settings = new GeneralViewModel();
        AeroColorRegistrySetting themeColor = new AeroColorRegistrySetting("Theme Color", "", "ColorizationColor");

        RegistrySettingsViewModel registrySettingsViewModel = new RegistrySettingsViewModel();
        AeroColorsViewModel aeroColorsViewModel = new AeroColorsViewModel();


        bool isHighContrast = false; //finish this feature
        
        public MainWindow()
        {
            InitializeComponent();
            
            UpdateFontList();
            LoadDPIScale();
            LoadAeroTab();
            LoadAeroColors();
            UpdateWallpaperInfo();
            LoadThemeName();
            LoadRegistrySettings();

            SelectItem(Settings.itemSettings[12]);
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

        
        void LoadAeroColors()
        {
            comboBoxAeroColors.DataContext = aeroColorsViewModel;
            aeroColorsViewModel.AddNoCheck("Active Window Color", "AccentColor");
            aeroColorsViewModel.AddNoCheck("Inactive Window Color", "AccentColorInactive");
        }

        
        void LoadRegistrySettings()
        {
            listBoxDMW.DataContext = registrySettingsViewModel;
            registrySettingsViewModel.Add("Show accent color on the title bars", "ColorPrevalence", new Version(10,0));
            registrySettingsViewModel.Add("Enable opaque taskbar (win 8 only)", "ColorizationOpaqueBlend", new Version(6, 2));
            //RegistrySettingsController.Add("Enable composition", "Composition", new Version(6, 1));           
            registrySettingsViewModel.Add("Enable peek at desktop", "EnableAeroPeek", new Version(6, 1));
            registrySettingsViewModel.Add("Hibernate Thumbnails", "AlwaysHibernateThumbnails", new Version(6, 1));
            registrySettingsViewModel.AddWithPath("Enable transparency effects", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "EnableTransparency", new Version(10, 0));
            registrySettingsViewModel.AddWithPath("Show accent color on the start and actioncenter", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "ColorPrevalence", new Version(10,0));
            registrySettingsViewModel.AddWithPath("Apps use light theme", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", new Version(10, 0));
            registrySettingsViewModel.AddWithPath("System uses light theme", @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", new Version(10, 0));
            registrySettingsViewModel.AddWithPath("Always show scrollbars in modern apps", @"Control Panel\Accessibility", "DynamicScrollbars", new Version(10,0));
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

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await SaveChanges();
        }

        string aeroStyle = "";

         async Task SaveChanges() {         
            if (!checkBoxOverwriteThemeStyle.IsChecked.Value)
            {
                isHighContrast = false;
                aeroStyle = "";
            }

            string themeName = textBoxThemeName.Text;
            string wallpaperPath = ""; //chyyba na to UI 

            
            ThemeSettings SaveTheme = Task.Run(() => new ThemeSettings(themeName, themeColor.ItemColor.Value, aeroStyle, wallpaperPath, itemSettings, fontSettings)).Result;
            
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

            await Task.Delay(2000);
            await registrySettingsViewModel.SaveAll();
            aeroColorsViewModel.SaveAll();
            KillDWM();
            MessageBox.Show("You need to restart to apply these changes.", "Restart required", MessageBoxButton.OK, MessageBoxImage.Information);
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

        /// <summary>
        /// restore all changes
        /// </summary>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void buttonAeroColor_Click(object sender, RoutedEventArgs e)
        {
            aeroColorsViewModel.ChangeColorCurrent((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem);
            ((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem).ItemColor = OpenColorDialog(((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem).ItemColor);
        }
    }
}
