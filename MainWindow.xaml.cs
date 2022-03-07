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
using AdvancedWindowsAppearence.Converters;
using System.Drawing;
using System.Windows.Threading;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GeneralViewModel Settings = new GeneralViewModel();
        ModernWindow ModernWindow = null;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Settings;
            UpdateFontList();
            LoadThemeName();

            //add backup & restore page
            restoreTabFrame.Content = new RestorePage(Settings);
        }

        private async void window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWindowsLayout(Environment.OSVersion.Version);

            Updater updater = new Updater();
            Version curVersion = updater.GetCurrentVersionInfo();
            versionNameTextBlock.Text = "Version: " + curVersion.ToString(2);

            bool isUpToDate = await updater.IsUpToDate();

            if (!isUpToDate)
            {
                var newVersion = await updater.GetNewVersionInfoAsync();
                MessageBoxResult result = MessageBox.Show($"New update is available: {newVersion.ToString(2)} (Current verson: {curVersion.ToString(2)}) \n\nDownload now?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {             
                    updater.DownloadUpdate();
                }
                versionNameTextBlock.Text += "\nUpdate available!";
            }            
        }

        void UpdateWindowsLayout(Version WinVer)
        {
            if (WinVer < new Version(6, 2))
            {
                LinearGradientBrush buttonBrush = new LinearGradientBrush(
                    new GradientStopCollection(
                        new GradientStop[]
                        {
                            new GradientStop(System.Windows.SystemColors.ControlColor, 0.5),
                            new GradientStop(System.Windows.SystemColors.ControlLightColor, 0.5)
                        }),
                    90
                    );
                App.Current.Resources["ButtonFaceColor"] = buttonBrush;
            }

            if (WinVer < new Version(10, 0)) //Windows 8.1 and less, keep standard UI
                return;
            
            //Windows 10/11 - create new modern window, close the old one
            ModernWindow = new ModernWindow(Settings.RegistrySettingsViewModel.RegistrySettings[5].Checked);

            ModernWindow.Owner = this;
            ModernWindow.Width = this.Width;
            ModernWindow.Height = this.Height;
            ModernWindow.contentFrame.Content = bgGrid;
            ModernWindow.Show();
            ModernWindow.Owner = null;

            if (WinVer >= new Version(10, 0, 22000)) //for Windows 11 - to round corners
                ModernWindow.RoundWindow(14d);
            Close();

        }

        void LoadThemeName()
        {
            ThemeSettings themeSettings = new ThemeSettings();
            string themeName = themeSettings.GetThemePath();

            themeName = themeName.Split("\\".ToCharArray()).Last();
            themeName = themeName.Replace(".theme", "");

            if (!themeName.Contains(" (Edited)"))
                Settings.ThemeName = themeName + " (Edited)";
            else
                Settings.ThemeName = themeName;

        }
        
        void UpdateFontList()
        {
            comboBoxFont.Items.Clear();
            List<System.Drawing.Font> fonts = FontManager.GetSystemFonts();
            foreach (var f in fonts)
            {
                comboBoxFont.Items.Add(f.Name);
            }
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
        }
        #endregion


        #region Fonts Tab

        private void buttonEditFontColor_Click(object sender, RoutedEventArgs e)
        {
            var selSetting = GetSelFontSetting();
            selSetting.ItemColor = OpenColorDialog(selSetting.ItemColor);
        }

        private void comboBoxFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(comboBoxFont.SelectedItem is string selection)
            {
                var selSetting = GetSelFontSetting();
                selSetting.ChangeFontName(selection);
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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
            if (checkBoxOverwriteThemeStyle.IsChecked.Value == false)
            {
                Settings.UpdateThemeStyle("");
                foreach(RadioButton rb in stackPanelAeroSettingsButtons.Children)
                {
                    rb.IsChecked = false;
                }
            }
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

        private void buttonOpenControlPanel_Click(object sender, RoutedEventArgs e)
        {
            Settings.ShowThemesControlPanel();
        }

        private void TextBlockOpenThemesFolder_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Settings.OpenThemesFolder();
        }

        #endregion


        #region Aero Tab


        private void ButtonColorSync_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selItem = (AeroColorRegistrySetting)btn.DataContext;
            if (selItem == null) return;
            selItem.ItemColor = Settings.ThemeColor.ItemColor;

        }

        private void buttonAeroColor_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selItem = (AeroColorRegistrySetting)btn.DataContext;
            if (selItem == null) return;
            selItem.ItemColor = OpenColorDialog(selItem.ItemColor);
        }

        private void buttonThemeColor_Click(object sender, RoutedEventArgs e)
        {
            Settings.ThemeColor.ItemColor = OpenColorDialog(Settings.ThemeColor.ItemColor);
        }

        private void textBoxColorOpacity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!textBoxColorOpacity.IsFocused)
                return;
            string opacityText = textBoxColorOpacity.Text;
            if (opacityText == null || opacityText=="")
                return;
            int alpha = int.Parse(opacityText);
            if (alpha > byte.MaxValue || alpha < byte.MinValue) return;
            Settings.ThemeColor.ItemColor = System.Drawing.Color.FromArgb(alpha, Settings.ThemeColor.ItemColor.Value.R, Settings.ThemeColor.ItemColor.Value.G, Settings.ThemeColor.ItemColor.Value.B);
        }
        #endregion


        #region Buttons Panel
        //Close Program
        private void CloseButton_Click(object sender, RoutedEventArgs e) 
        {
            Application.Current.Shutdown();
        }

        private async void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            if (saveChangesComboBox.Text == "Apply as theme") 
                Settings.UseThemes = true;
            else
                Settings.UseThemes = false;
            await Settings.SaveChanges();

            await Task.Delay(200);
            if(ModernWindow != null)
            {
                Dispatcher.Invoke(() => ModernWindow.UpdateTheme(Settings.RegistrySettingsViewModel.RegistrySettings[5].Checked.Value));
            }
                       
        }

        #endregion

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var githubProcess = new Process();
            githubProcess.StartInfo = new ProcessStartInfo("https://github.com/ArkadySK/AdvancedWindowsAppearance");
            githubProcess.Start();
        }
    }
}
