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
using AdvancedWindowsAppearence.Previews;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public const int PreviewWidth = 200;
        GeneralViewModel Settings;
        ModernWindow ModernWindow = null;
        bool allUILoadedAtStart = false;

        #region initialization

        public MainWindow()
        {
            InitializeComponent();
            Settings = new GeneralViewModel();

            UpdateFontList();
            LoadThemeName();

            allUILoadedAtStart = Settings.ApplicationSettings.ShowAllUI;
            this.DataContext = Settings;

        }

        void LoadAdvancedUI()
        {
            Dispatcher.Invoke(() => 
            {
                //add backup & restore page
                restoreTabFrame.Content = new RestorePage(Settings);

                //add wallpaper, slideshow and color bg pages
                WallpaperFrame.Content = new WallpaperSelectionPage(Settings.WallpaperSettings);
                SlideshowFrame.Content = new SlideshowSettingsPage(Settings.WallpaperSettings);
                ColorFrame.Content = new ColorBackgroundSelectionPage(Settings.WallpaperSettings);

                Settings.WallpaperSettings.UpdateWallpaperType();
                UpdateWallpaperTypeComboBox();
            });
        }

        private async void window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWindowsLayout(Environment.OSVersion.Version);

            Updater updater = new Updater();
            Version curVersion = updater.GetCurrentVersionInfo();
            versionNameTextBlock.Text = "Version: " + curVersion.ToString(2);

            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show("Error checking for update: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            
            if (!Settings.ApplicationSettings.ShowAllUI)
            {
                MainTabControl.SelectedIndex = 2;
                return;
            }
            

            await Task.Run(LoadAdvancedUI);
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
            Settings.IsWindows10 = (bool)(WinVer >= new Version(10, 0));
            if (!Settings.IsWindows10 || Settings.ApplicationSettings.UseNativeTitlebar) //Windows 8.1 and less, keep standard UI
                return;
            
            //Windows 10/11 - create new modern window, close the old one
            ModernWindow = new ModernWindow(Settings.RegistrySettingsViewModel.RegistrySettings[5].Checked);

            ModernWindow.Owner = this;
            ModernWindow.Width = this.Width;
            ModernWindow.Height = this.Height;
            ModernWindow.contentFrame.Content = bgGrid;
            ModernWindow.Show();
            ModernWindow.Owner = null;
            Close();

        }

        #endregion

        
        #region Theme tab
        void LoadThemeName()
        {
            string themeName = ThemeSettings.GetThemePath();

            themeName = themeName.Split("\\".ToCharArray()).Last();
            themeName = themeName.Replace(".theme", "");

            if (!themeName.Contains(" (Edited)"))
                Settings.ThemeName = themeName + " (Edited)";
            else
                Settings.ThemeName = themeName;

        }
        #endregion


        #region Colors and Metrics and Fonts commmon functions

        void UpdateFontList()
        {
            comboBoxFont.Items.Clear();
            List<System.Drawing.Font> fonts = FontManager.GetSystemFonts();
            foreach (var f in fonts)
            {
                if (f is null) continue;
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


        private void openClassicWindowTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClassicWindowForm classicWindowForm = new ClassicWindowForm();
            classicWindowForm.ShowDialog();
        }

        public static System.Drawing.Color OpenColorDialog(System.Drawing.Color? defaultCol)
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
        #endregion


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

        private void buttonMsstylesFileDialog_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleButtonAero_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateThemeStyle("Aero\\Aero", false);
        }

        private void ToggleButtonAeroLite_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateThemeStyle("Aero\\AeroLite", false);
        }

        private void ToggleButtonHighContrast_Click(object sender, RoutedEventArgs e)
        {
            Settings.UpdateThemeStyle("Aero\\AeroLite", false);
        }

        private void CustomStyleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.UpdateThemeStyle(((TextBox)sender).Text, true);
        }

        private void CheckBoxOverwriteThemeStyle_Click(object sender, RoutedEventArgs e)
        {
            
            stackPanelAeroSettingsButtons.IsEnabled = checkBoxOverwriteThemeStyle.IsChecked.Value;
            if (checkBoxOverwriteThemeStyle.IsChecked.Value == false)
            {
                Settings.UpdateThemeStyle("", true);
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
                Settings.UpdateThemeStyle("", true);
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


        #region Wallpaper Tab

        void UpdateWallpaperTypeComboBox()
        {
            WallpaperFrame.Visibility = Visibility.Collapsed;
            SlideshowFrame.Visibility = Visibility.Collapsed;
            ColorFrame.Visibility = Visibility.Collapsed;
            switch (Settings.WallpaperSettings.WallpaperType)
            {
                case WallpaperSettings.WallpaperTypes.Image:
                    {
                        WallpaperFrame.Visibility = Visibility.Visible;
                        break;
                    }
                case WallpaperSettings.WallpaperTypes.Slideshow:
                    {
                        SlideshowFrame.Visibility = Visibility.Visible;
                        break;
                    }
                default:
                    {
                        ColorFrame.Visibility = Visibility.Visible;
                        break;
                    }
            }
        }

        private void WallpaperTypeComboBox_DropDownClosed(object sender, EventArgs e)
        {
            //the UI is delayed, so it will display changes correctly
            UpdateWallpaperTypeComboBox();
        }

        #endregion


        #region Buttons Panel

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var githubProcess = new Process();
            githubProcess.StartInfo = new ProcessStartInfo("https://github.com/ArkadySK/AdvancedWindowsAppearance");
            githubProcess.Start();
        }

        #endregion


        #region Window properties

        /// <summary>
        /// change size and adjust layout if needed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (bgGrid.RenderSize.Width < 750)
            {
                opaqueRectangle.Width = bgGrid.ActualWidth;
                MainTabControl.TabStripPlacement = Dock.Top;
                SidepanelButtomTextStackPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                opaqueRectangle.Width = bgGrid.ActualWidth - 320;
                MainTabControl.TabStripPlacement = Dock.Left;
                SidepanelButtomTextStackPanel.Visibility = Visibility.Visible;
            }
        }
        #endregion


        #region Save
        //updating the theme
        async Task UpdateTheme() 
        {
            await Task.Delay(200);
            if (ModernWindow != null)
            {
                Dispatcher.Invoke(() => ModernWindow.UpdateTheme(Settings.RegistrySettingsViewModel.RegistrySettings[5].Checked.Value));
            }
        }


        private async void SaveThemeAsTheme_Click(object sender, RoutedEventArgs e)
        {
            await Settings.SaveThemeAsTheme();
            await UpdateTheme();
        }


        private async void SaveTitleColorsAsTheme_Click(object sender, RoutedEventArgs e)
        {
            try { 
            await Settings.SaveTitleColorsAsTheme();
            }       
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SaveTitleColorsToRegistry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveTitleColorsToRegistry();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private async void SaveColorsMetricsAsTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveColorsMetricsAsTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SaveColorsMetricsToRegistry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveColorsMetricsToRegistry();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private async void SaveFontsAsTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveFontsAsTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SaveFontsToRegistry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveFontsToRegistry();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private async void SaveWallpaperAsTheme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveWallpaperAsTheme();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void SaveWallpaperToRegistry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Settings.SaveWallpaperToRegistry();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Settings.IsSavingInProgress = false;
            }
        }


        private async void SaveRegistrySettingsRegistry_Click(object sender, RoutedEventArgs e)
        {
            await Settings.SaveRegistrySettingsToRegistry();
            await UpdateTheme();
        }

        #endregion


        #region UI type checkbox
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!allUILoadedAtStart)
            {
                LoadAdvancedUI();
                allUILoadedAtStart = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedIndex = 2;
        }
        #endregion


        #region Restart buttons
        private void buttonRestartExplorer_Click(object sender, RoutedEventArgs e)
        {
            GeneralViewModel.RestartExplorer();
        }

        private void buttonRestartDwm_Click(object sender, RoutedEventArgs e)
        {
            GeneralViewModel.RestartDWM();
        }
        #endregion

    }
}
