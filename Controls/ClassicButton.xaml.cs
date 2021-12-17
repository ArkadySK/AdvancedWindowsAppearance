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




        static readonly DependencyProperty TopLeftEdgeColorProperty
            = DependencyProperty.Register(nameof(TopLeftEdgeColor), typeof(Brush), typeof(ClassicButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(10, 100, 200)) ));
        public Brush TopLeftEdgeColor
        {
            get { return (Brush)GetValue(TopLeftEdgeColorProperty); }
            set { SetValue(TopLeftEdgeColorProperty, value); }
        }

        static readonly DependencyProperty BottomRightEdgeColorProperty
            = DependencyProperty.Register(nameof(BottomRightEdgeColor), typeof(Brush), typeof(ClassicButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(10, 100, 200))));
        public Brush BottomRightEdgeColor
        {
            get { return (Brush)GetValue(BottomRightEdgeColorProperty); }
            set { SetValue(BottomRightEdgeColorProperty, value); }
        }

        static readonly DependencyProperty TopLeftColorProperty
           = DependencyProperty.Register(nameof(TopLeftColor), typeof(Brush), typeof(ClassicButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(10, 100, 200))));
        public Brush TopLeftColor
        {
            get { return (Brush)GetValue(TopLeftColorProperty); }
            set { SetValue(TopLeftColorProperty, value); }
        }

        static readonly DependencyProperty BottomRightColorProperty
            = DependencyProperty.Register(nameof(BottomRightColor), typeof(Brush), typeof(ClassicButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(10, 100, 200))));
        public Brush BottomRightColor
        {
            get { return (Brush)GetValue(BottomRightColorProperty); }
            set { SetValue(BottomRightColorProperty, value); }
        }


        public ClassicButton()
        {
            InitializeComponent();
        }
    }
}
