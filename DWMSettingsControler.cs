using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedWindowsAppearence
{
    public class DWMSettingsControler
    {
        public ObservableCollection<DWMSetting> DWMSettings { get; set; } = new ObservableCollection<DWMSetting>();

        public void Add(string name, string path, Version winVer)
        {
            if (winVer > Environment.OSVersion.Version) return;
            DWMSetting dWMSetting = new DWMSetting(name, path);
            DWMSettings.Add(dWMSetting);
        }
        public void SaveAll()
        {
            foreach (DWMSetting dWMSetting in DWMSettings)
            {
                if (dWMSetting == null) continue;
                if (!dWMSetting.Checked.HasValue) continue;

                dWMSetting.SaveToRegistry();
            }
        }
    }
}
