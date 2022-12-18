using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class AeroColorsViewModel
    {
        public ObservableCollection<AeroColorRegistrySetting> AeroColors { get; set; } = new ObservableCollection<AeroColorRegistrySetting>();
        public void Add(string name, string registrykey, Version winVer)
        {
            var registryPath = @"Software\Microsoft\Windows\DWM";
            AddWithPath(name, registryPath, registrykey, winVer);
        }
        public void AddWithPath(string name, string registrypath, string registrykey, Version winVer)
        {
            if (winVer > Environment.OSVersion.Version) return;
            AeroColorRegistrySetting setting = new AeroColorRegistrySetting(name, registrypath, registrykey);
            AeroColors.Add(setting);
        }

        public void RemoveFromRegistry(AeroColorRegistrySetting setting)
        {
            setting.RemoveFromRegistry();
        }

        public async Task SaveAll()
        {
            List<Task> tasks = new List<Task>();
            foreach (AeroColorRegistrySetting setting in AeroColors)
            {
                if (setting == null) continue;
                if (!setting.Enabled) RemoveFromRegistry(setting);
                else
                    tasks.Add(Task.Run(() => setting.SaveToRegistry()));
            }
            await Task.WhenAll(tasks);
        }

        public string GetSettingsInReg()
        {
            string output = @"[HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM]";
            foreach (var ac in AeroColors)
            {
                output += "\n\"" + ac.RegistryKey + "\"=dword:" + int.Parse(ac.ItemColorValue).ToString("X");
            }
            return output;
        }
    }
}
