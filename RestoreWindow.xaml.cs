using System;
using System.Collections.Generic;
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
    public partial class RestoreWindow : Window
    {
        GeneralViewModel Settings;
        public RestoreWindow(GeneralViewModel generalViewModel)
        {
            InitializeComponent();
            Settings = generalViewModel;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();    
        }

        async Task Restore()
        {
            MessageBoxResult result = MessageBox.Show("Restore all settings related to advanced theming? \n\nA restart will be required.", "Are you sure?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            await Settings.ResetToDefaults();
            MessageBox.Show("Colors, sizes and fonts were restored. \n\nPlease restart the computer.\nProgram will now close.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        int selIndex;

        private void buttonConfirm_Click(object sender, RoutedEventArgs e)
        {
            switch (selIndex)
            {
                case 0:
                    Settings.ResetDWM();
                    break;
                case 1:
                    Settings.ResetTheme();
                    break;
                case 2:
                    Settings.ResetColors();
                    break;
                case 3:
                    Settings.ResetFonts();
                    break;
            }
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
    }
}
