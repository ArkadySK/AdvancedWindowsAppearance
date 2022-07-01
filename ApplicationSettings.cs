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
        public static bool GetUIType()
        {
            int value = (int)GetValue("UIType", 0);
            return value == 0? false : true; 
        }

        public static void SetUIType(bool value)
        {
            SetValue("UIType", value, RegistryValueKind.DWord);
        }

        private static void SetValue(string name, object value, RegistryValueKind registryValueKind)
        {
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\AWA", true);
            if (key == null)
            {
                key.Close();
                return;
            }
            key.SetValue(name, value, registryValueKind);
            key.Close();
        }
        public static object GetValue(string name, object defaultValue) { 
            var key = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);
            key.CreateSubKey("AWA", RegistryKeyPermissionCheck.Default);
            key.Close();

            var key2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\AWA", true);
            object value = defaultValue;
            try
            {
                value = (int)key2.GetValue(name, 0);
            }
            catch { }
            key2.Close();
            return value;
        }
    }
}
