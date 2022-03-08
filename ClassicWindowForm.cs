using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedWindowsAppearence
{
    public partial class ClassicWindowForm : Form
    {
        [DllImport("uxtheme.dll")]
        private static extern int SetWindowTheme(IntPtr hWnd, string appname, string idlist);

        public ClassicWindowForm()
        {
            InitializeComponent();
            SetWindowTheme(this.Handle, "", "");
            
        }
    }
}
