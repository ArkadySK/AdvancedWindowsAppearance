using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    class AeroColorsViewModel
    {
        public ObservableCollection<AeroColorRegistrySetting> AeroColors { get; set; } = new ObservableCollection<AeroColorRegistrySetting>();

        public void Add(string name, string registrykey)
        {
            var winVer = new Version(6,2);
            var registryPath = @"Software\Microsoft\Windows\DWM";
            AddWithPath(name, registryPath, registrykey, winVer);
        }

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

        public void SaveAll()
        {
            foreach (AeroColorRegistrySetting setting in AeroColors)
            {
                if (setting == null) continue;
                if (!setting.Enabled) RemoveFromRegistry(setting);
                else
                setting.SaveToRegistry();
            }
        }
        public void ChangeColorCurrent(AeroColorRegistrySetting aerocolor)
        {

        }
    }
}
