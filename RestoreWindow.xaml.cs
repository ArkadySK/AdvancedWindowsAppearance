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
        public RestoreWindow()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Close();    
        }
    }
}
