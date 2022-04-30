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
    /// Interaction logic for ScreenslideSettingsPage.xaml
    /// </summary>
    public partial class SlideshowSettingsPage : Page
    {
        WallpaperSettings WallpaperSettings;
        public SlideshowSettingsPage(WallpaperSettings wallpaperSettings)
        {
            WallpaperSettings = wallpaperSettings;
            if (WallpaperSettings.Slideshow == null)
                WallpaperSettings.CreateDefaultSlideshow();
            InitializeComponent();
            DataContext = WallpaperSettings;
        }

        private void ChangeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            WallpaperSettings.Slideshow.ShowFolderDialogSlideshow();
            ImagesListView.ItemsSource = WallpaperSettings.Slideshow.FolderImages;
        }
    }
}
