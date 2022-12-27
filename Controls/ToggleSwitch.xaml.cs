using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdvancedWindowsAppearence.Controls
{
    /// <summary>
    /// Interaction logic for ToggleSwitch.xaml
    /// </summary>
    public partial class ToggleSwitch : UserControl, INotifyPropertyChanged
    {

        static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ToggleSwitch), new PropertyMetadata(default(string)));

        static readonly DependencyProperty IsOnProperty
            = DependencyProperty.Register(nameof(IsOn), typeof(bool), typeof(ToggleSwitch), new PropertyMetadata(default(bool)));

        static readonly DependencyProperty FillProperty
            = DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(ToggleSwitch), new PropertyMetadata(default(Brush)));

        public event PropertyChangedEventHandler PropertyChanged;

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set
            {
                SetValue(TextProperty, value);
                NotifyPropertyChanged();
            }
        }

        public bool IsOn
        {
            get => (bool)GetValue(IsOnProperty);
            set
            {
                SetValue(IsOnProperty, value);
                NotifyPropertyChanged();
                UpdateState();
            }
        }

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set
            {
                SetValue(FillProperty, value);
                NotifyPropertyChanged();
            }
        }

        public ToggleSwitch()
        {
            InitializeComponent();
        }

        internal void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void switchBorder_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateState();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            switchBorder.Opacity = 0.65;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            switchBorder.Opacity = 1.0;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsOn = !IsOn;
        }

        private void UpdateState()
        {
            if (IsOn)
            {
                switchEllipse.HorizontalAlignment = HorizontalAlignment.Right;
                switchBorder.Background = Fill;
                switchBorder.BorderBrush = Fill;
                switchEllipse.Fill = Brushes.White;
            }
            else
            {
                switchEllipse.HorizontalAlignment = HorizontalAlignment.Left;
                switchBorder.BorderBrush = Brushes.Black;
                switchBorder.Background = null;
                switchEllipse.Fill = Brushes.Black;
            }
        }

    }
}
