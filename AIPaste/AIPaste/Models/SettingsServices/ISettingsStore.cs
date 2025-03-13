using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.DataModels;

namespace AIPaste.Models.SettingsServices
{
    public interface ISettingsStore
    {
        AppSettings LoadSettings();
        void SaveSettings(AppSettings appSettings);
        AppSettings ResetSettings();
    }
}
