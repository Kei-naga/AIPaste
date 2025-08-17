using AIPaste.DTO.SettingsDTO;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPaste.Models.SettingsServices
{
    public interface ISettingsService
    {
        IAppSettings LoadSettings();
        void SaveSettings(IAppSettings appSettings);
        IAppSettings ResetSettings();
    }
}
