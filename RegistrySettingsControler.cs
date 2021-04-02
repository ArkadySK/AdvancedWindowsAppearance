using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class RegistrySettingsControler
    {
        public ObservableCollection<RegistrySetting> RegistrySettings { get; set; } = new ObservableCollection<RegistrySetting>();

        public void Add(string name, string registrykey, Version winVer)
        {
            var registryPath = @"Software\Microsoft\Windows\DWM";
            AddWithPath(name, registryPath, registrykey, winVer);
        }
        public void AddWithPath(string name, string registrypath, string registrykey, Version winVer)
        {
            if (winVer > Environment.OSVersion.Version) return;
            RegistrySetting registrySetting = new RegistrySetting(name, registrypath, registrykey);
            RegistrySettings.Add(registrySetting);
        }
        public void SaveAll()
        {
            foreach (RegistrySetting registrySetting in RegistrySettings)
            {
                if (registrySetting == null) continue;
                if (!registrySetting.Checked.HasValue) continue;

                registrySetting.SaveToRegistry();
            }
        }
    }
}
