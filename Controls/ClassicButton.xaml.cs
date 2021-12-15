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

namespace AdvancedWindowsAppearence.Controls
{
    /// <summary>
    /// Interaction logic for ClassicButton.xaml
    /// </summary>
    public partial class ClassicButton : UserControl
    {
        static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ClassicButton), new PropertyMetadata(default(string)));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        static readonly DependencyProperty ButtonUpTopColorProperty
            = DependencyProperty.Register(nameof(ButtonUpTopColor), typeof(Brush), typeof(ClassicButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(10, 100, 200)) ));
        public Brush ButtonUpTopColor
        {
            get { return (Brush)GetValue(ButtonUpTopColorProperty); }
            set { SetValue(ButtonUpTopColorProperty, value); }
        }

        static readonly DependencyProperty ButtonBottomRightColorProperty
            = DependencyProperty.Register(nameof(ButtonBottomRightColor), typeof(Brush), typeof(ClassicButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(10, 100, 200))));
        public Brush ButtonBottomRightColor
        {
            get { return (Brush)GetValue(ButtonBottomRightColorProperty); }
            set { SetValue(ButtonBottomRightColorProperty, value); }
        }

        public ClassicButton()
        {
            InitializeComponent();
        }
    }
}
