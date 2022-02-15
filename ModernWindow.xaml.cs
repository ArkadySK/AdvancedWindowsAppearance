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
    /// Interaction logic for ModernWindow.xaml
    /// </summary>
    public partial class ModernWindow : Window
    {
        public ModernWindow()
        {
            InitializeComponent();
        }
        void SetTransparency()
        {
            WindowBlur.SetIsEnabled(this, true);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetTransparency();
        }

        private void window_Activated(object sender, EventArgs e)
        {
            //titlebarGrid.Background = BrushToColorConverter.MediaColorToBrush(Settings.AeroColorsViewModel.AeroColors[0].ItemColor);
            this.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void window_Deactivated(object sender, EventArgs e)
        {           
            this.Background = System.Windows.Media.Brushes.Gray;
            /*
            titlebarGrid.Background = System.Windows.Media.Brushes.Transparent;
            

            var inactiveAeroColor = Settings.AeroColorsViewModel.AeroColors[1];
            if (inactiveAeroColor == null)
            {
                titlebarGrid.Background = System.Windows.Media.Brushes.White;
                return;
            }

            if(!inactiveAeroColor.Enabled)
                titlebarGrid.Background = System.Windows.Media.Brushes.White;

            else
                titlebarGrid.Background = BrushToColorConverter.MediaColorToBrush(inactiveAeroColor.ItemColor);*/
        }



        #region Caption Buttons

        void Minimize()
        {
            SystemCommands.MinimizeWindow(this);
        }

        void Maximize()
        {
            bgGrid.Margin = new Thickness(5);
            SystemCommands.MaximizeWindow(this);
            maximizeButton.Content = "";
        }

        void Restore()
        {
            bgGrid.Margin = new Thickness(1);
            SystemCommands.RestoreWindow(this);
            maximizeButton.Content = "";
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;
            if (this.WindowState != WindowState.Normal)
            {
                Restore();
                return;
            }
            this.DragMove();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            Minimize();
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState != WindowState.Maximized)
            {
                Maximize();
            }
            else
            {
                Restore();
            }


        }

        private void closeButton_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void contentFrame_ContentRendered(object sender, EventArgs e)
        {
            MessageBox.Show("a");
            object defaultScrollViewer = contentFrame.FindName("defaultScrollViewer");
            if(defaultScrollViewer is ScrollViewer sv)
            opaqueRectangle.Width = sv.ActualWidth;            
        }
    }
}
