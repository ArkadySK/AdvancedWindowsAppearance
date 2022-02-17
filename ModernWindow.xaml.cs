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
using System.Windows.Shell;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Interaction logic for ModernWindow.xaml
    /// </summary>
    public partial class ModernWindow : Window
    {
        bool IsLightMode = true;

        public ModernWindow(bool? isLightMode)
        {
            if(isLightMode != null)
                IsLightMode = isLightMode.Value;

            InitializeComponent();
        }

        /// <summary>
        /// Update Ui by the selected theme
        /// </summary>
        public void UpdateTheme(bool isLightMode)
        {
            if (!isLightMode) //force dark mode
            {
                App.Current.Resources["ButtonFaceColor"] = new BrushConverter().ConvertFromString("#FF404040");
                App.Current.Resources["BackgroundColor"] = Brushes.Black;
                App.Current.Resources["BackgroundColorTabItems"] = new BrushConverter().ConvertFromString("#9F252525");
                App.Current.Resources["ForegroundColor"] = Brushes.White;
            }
            else
            {
                App.Current.Resources["ButtonFaceColor"] = SystemColors.ControlBrush;
                App.Current.Resources["BackgroundColor"] = SystemColors.WindowBrush;
                App.Current.Resources["BackgroundColorTabItems"] = new BrushConverter().ConvertFromString("#BFFFFFFF");
                App.Current.Resources["ForegroundColor"] = SystemColors.WindowTextBrush;
            }
            IsLightMode = isLightMode;
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
            this.Background = System.Windows.Media.Brushes.Transparent;
            titlebarGrid.Opacity = 1;
            windowBorder.BorderBrush = (Brush)App.Current.Resources["ThemeColor"];
            windowBorder.Opacity = 1;
        }

        private void window_Deactivated(object sender, EventArgs e)
        {
            if (IsLightMode)
                this.Background = System.Windows.Media.Brushes.Gray;
            else
                this.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF101010");
            titlebarGrid.Opacity = 0.4;
            windowBorder.BorderBrush = Brushes.Gray;
            windowBorder.Opacity = 0.4;
        }


        #region Caption Buttons

        void Minimize()
        {
            SystemCommands.MinimizeWindow(this);
        }

        void Maximize()
        {
            masterGrid.Margin = new Thickness(5);
            SystemCommands.MaximizeWindow(this);
            maximizeButton.Content = "";
        }

        void Restore()
        {
            masterGrid.Margin = new Thickness(0);
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

        public void RoundWindow(double radius) {
            var windowChrome = WindowChrome.GetWindowChrome(this);
            windowChrome.CornerRadius = new CornerRadius(radius);
            windowBorder.CornerRadius = new CornerRadius(radius / 2);
        }

        private void contentFrame_ContentRendered(object sender, EventArgs e)
        {
            /* in case this is used in a usefull situation
            Grid grid = (Grid)contentFrame.Content;
            TabControl tabControl = (TabControl)grid.Children[1]; 
            
            //ScrollViewer defaultScrollView = (ScrollViewer) (tabControl.Items[0] as TabItem).Content;
            Rectangle opaqueRectangle = (Rectangle)grid.Children[0];
            */
            UpdateTheme(IsLightMode);
        }
    }
}
