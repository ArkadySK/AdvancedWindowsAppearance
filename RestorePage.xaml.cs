using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Interaction logic for RestoreWindow.xaml
    /// </summary>
    public partial class RestorePage : Page
    {
        GeneralViewModel Settings;
        readonly string savePath = Environment.CurrentDirectory + @"\Exported Settings";

        public RestorePage(GeneralViewModel generalViewModel)
        {
            InitializeComponent();
            Settings = generalViewModel;
        }
        async Task Restore()
        {
            MessageBoxResult result = MessageBox.Show("Restore all settings related to advanced theming? \n\nA restart will be required.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            await Settings.ResetToDefaults();
            MessageBox.Show("Colors, sizes and fonts were restored. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        int selIndex = -1;

        private async void buttonConfirm_Click(object sender, RoutedEventArgs e)
        {
            switch (selIndex)
            {
                case 0:
                    await Settings.ResetDWM();
                    MessageBox.Show("Desktop window manager settings were reseted. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 1:
                    await Settings.ResetTheme();
                    MessageBox.Show("Default theme applied successfully. \n\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 2:
                    await Settings.ResetColors();
                    MessageBox.Show("Colors settings were restored. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 3:
                    await Settings.ResetFonts();
                    MessageBox.Show("Sizes and fonts were restored. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case 4:
                    await Settings.ResetToDefaults();
                    MessageBox.Show("DWM settings, colors, sizes and fonts were restored. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                default:
                    MessageBox.Show("No option selected! \n\nPlease select an option.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
            }
            Application.Current.Shutdown();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            int i = -1;
            foreach(RadioButton rb in stackPanelBullets.Children)
            {
                i++;
                if(rb.Content == (sender as RadioButton).Content )
                    selIndex = i;
                else
                    rb.IsChecked = false;
            }

        }

        private async void exportToReg_Click(object sender, RoutedEventArgs e)
        {
            await Settings.ExportToReg();
            MessageBox.Show(("Export was successful!\nThe exported file is saved in: " + savePath + Environment.NewLine + "You can restore the settings just by opening the settings-export file."), "Export Finished", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void openRestoreFolder_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "explorer";
            startInfo.Arguments = savePath;
            Process.Start(startInfo);
        }

        private void importReg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "registry files|*.reg";
            dialog.Title = "Choose a restore file";
            dialog.InitialDirectory = savePath;

            var result = dialog.ShowDialog();
            
            if (result.Value == false)
            return;
            
            string fileName = dialog.SafeFileName;
            
            var confirmResult = MessageBox.Show("Are you sure to load settings from this file?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmResult == MessageBoxResult.No)
                return;
            Settings.RunRegFile(savePath + "\\" + fileName);

            MessageBox.Show("Settings restored successfully. \n\nThe program will now close. You should restart the device to apply changes.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            App.Current.Shutdown();
        }
    }
}
