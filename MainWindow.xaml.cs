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
        
        public MainWindow()
        {
            InitializeComponent();
            AssignDataContexts();
            UpdateFontList();
            UpdateWallpaperInfo();
            LoadThemeName();
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

        void AssignDataContexts()
        {
            this.DataContext = Settings;
            //to do:
            //title colors
            //theme settings
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
            ImageWallpaper.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(Settings.Wallpaper.Path);
            ImageWallpaper.Stretch = Stretch.UniformToFill;
        }

        ColorAppearanceSetting GetSelItemSetting()
        {
            return (ColorAppearanceSetting)comboBoxItems.SelectedItem; 
        }

        FontAppearanceSetting GetSelFontSetting()
        {
            return (FontAppearanceSetting)comboBoxFonts.SelectedItem;

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

        private void buttonEditItemColor_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelItemSetting();
            selSetting.ItemColor = OpenColorDialog(selSetting.ItemColor);
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

        private void comboBoxFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selSetting = GetSelFontSetting();
            if (selSetting == null) return;
            UpdateFontPreview(selSetting.Font, selSetting.Size.GetValueOrDefault(0), selSetting.IsBold, selSetting.IsItalic, selSetting.FontColor);
        }

        private void buttonEditFontColor_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelFontSetting();
            selSetting.FontColor = OpenColorDialog(selSetting.FontColor);
            selSetting.IsEdited = true;
            UpdateFontPreview(selSetting.FontColor.Value);
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
            UpdateFontPreview(selSetting.Font, selSetting.Size.GetValueOrDefault(0));
        }

        private void comboBoxFontSize_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxFonts.SelectedIndex == -1) return;
            var s = GetSelFontSetting();
            int size = int.Parse(comboBoxFontSize.Text);
            s.Size = size;
            UpdateFontPreview(s.Font, size);
        }

        void UpdateFontPreview(System.Drawing.Font f, float size, bool isBold, bool isItalic, System.Drawing.Color? textCol)
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
            textBlockPreview.Foreground = BrushToColor.MediaColorToBrush(textCol);
        }

        void UpdateFontPreview(System.Drawing.Font f, float size)
        {
            textBlockPreview.Content = "The quick brown fox jumps over the lazy dog";
            textBlockPreview.FontFamily = new System.Windows.Media.FontFamily(f.FontFamily.Name);
            textBlockPreview.FontSize = size * Settings.DPI;
        }


        #endregion


        #region Theme Style
        private void ToggleButtonAero_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateThemeStyle("Aero\\Aero");
        }

        private void ToggleButtonAeroLite_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateThemeStyle("Aero\\AeroLite");
        }

        private void ToggleButtonHighContrast_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateThemeStyle("Aero\\AeroLite");
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
                Settings.UpdateThemeStyle("");
            }
            else if (checkBoxOverwriteThemeStyle.IsChecked.Value)
                checkBoxOverwriteThemeStyle.IsChecked = true;

        }
        #endregion


        #region Aero Tab
        private void buttonThemeColor_Click(object sender, RoutedEventArgs e)
        {
            Settings.ThemeColor.ItemColor = OpenColorDialog(Settings.ThemeColor.ItemColor);           
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
            Settings.ThemeColor.ItemColor = System.Drawing.Color.FromArgb(alpha , Settings.ThemeColor.ItemColor.Value.R, Settings.ThemeColor.ItemColor.Value.G, Settings.ThemeColor.ItemColor.Value.B);
        }
        #endregion


        #region Buttons Panel
        //Close Program
        private void CloseButton_Click(object sender, RoutedEventArgs e) 
        {
            Close();
        }

        // restore all changes done to registry (colors, fonts, sizes) 
        private async void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Restore all settings related to advanced theming? \n\nA restart will be required.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;
            
            await Settings.ResetToDefaults();
            MessageBox.Show("Colors, sizes and fonts were restored. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);         
        }

        //check if this works
        private void buttonAeroColor_Click(object sender, RoutedEventArgs e)
        {
            Settings.AeroColorsViewModel.ChangeColorCurrent((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem);
            ((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem).ItemColor = OpenColorDialog(((AeroColorRegistrySetting)comboBoxAeroColors.SelectedItem).ItemColor);
        }

        private async void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            if (saveChangesComboBox.Text == "Apply as theme") 
                Settings.UseThemes = true;
            else
                Settings.UseThemes = false;
            await Settings.SaveChanges();
        }
        #endregion

    }
}
