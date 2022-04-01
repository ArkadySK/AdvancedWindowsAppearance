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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdvancedWindowsAppearence.Previews
{
    /// <summary>
    /// Interaction logic for WallpaperSelectionPage.xaml
    /// </summary>
    public partial class WallpaperSelectionPage : Page
    {
        GeneralViewModel Settings;
        public WallpaperSelectionPage(GeneralViewModel generalViewModel)
        {
            InitializeComponent();
            Settings = generalViewModel;
            DataContext = Settings;
        }

        private void changeWallpaper_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "Supported Image Files (*.png, *.jpg, *.bmp)|*.png;*.jpg;*.bmp|All files (*.*)|*.*";
            dialog.Title = "Select new wallpaper image";
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;
                Settings.Wallpaper.SetWallpaper(path);
            }

        }

    }
}
