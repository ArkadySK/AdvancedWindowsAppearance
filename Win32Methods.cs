using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsInteropServices
{
    internal class Win32Methods
    {
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        // The method to update desktop wallpaper in this case
        public static void UpdateWallpaper(string path)
        {
            const int SPI_SETDESKWALLPAPER = 20;
            const int SPIF_UPDATEINIFILE = 0x01;
            const int SPIF_SENDWININICHANGE = 0x02;

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
            0,
            path,
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }


        /// <summary>
        /// converts an item identifier list to a file system path
        /// </summary>
        /// <param name="pidl">the address of the path</param>
        /// <param name="pszPath">the output string it will save into</param>
        /// <returns></returns>

        [DllImport("shell32.dll")]
        public static extern Int32 SHGetPathFromIDList(
            IntPtr pidl,
            StringBuilder pszPath
        );


        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr ILCreateFromPath([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath);

    }
}
