﻿using System;
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
        
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = Settings;
            UpdateFontList();
            LoadThemeName();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateWindowsLayout(Environment.OSVersion.Version);
        }

        void UpdateWindowsLayout(Version WinVer)
        {
            if (WinVer <= new Version(6, 3)) //Windows 8.1 and less, keep standard UI
                return;
            
            //Windows 10/11 - create new modern window, close the old one
            ModernWindow modernWindow = new ModernWindow(Settings.RegistrySettingsViewModel.RegistrySettings[5].Checked);

            modernWindow.Owner = this;
            modernWindow.Width = this.Width;
            modernWindow.Height = this.Height;
            modernWindow.contentFrame.Content = bgGrid;
            modernWindow.Show();
            modernWindow.Owner = null;

            if (WinVer >= new Version(10, 0, 22000)) //for Windows 11 - to round corners
                modernWindow.RoundWindow(14d);
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

        // restore all changes done to registry (colors, fonts, sizes) 
        private void ButtonRestore_Click(object sender, RoutedEventArgs e)
        {
            RestoreWindow restoreWindow = new RestoreWindow(Settings);
            try
            {
                restoreWindow.Owner = this;
            }
            catch { }
            restoreWindow.ShowDialog();
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
