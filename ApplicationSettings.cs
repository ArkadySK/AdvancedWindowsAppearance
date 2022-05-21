using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    /// <summary>
    /// Class that manages AWA's settings, saves all data to registry key: Computer\HKEY_CURRENT_USER\SOFTWARE\AWA
    /// </summary>
    public static class ApplicationSettings
    {
        static bool GetUIType()
        {
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE");
            key.CreateSubKey("AWA");
            key = key.OpenSubKey("AWA");
            var value = (int)key.GetValue("UIType", 0);
            return value == 0? false : true; 
        }

        static void SetUIType(bool value)
        {
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
            key.CreateSubKey("AWA");
            key.SetValue("UIType", value);
        }
    }
}
