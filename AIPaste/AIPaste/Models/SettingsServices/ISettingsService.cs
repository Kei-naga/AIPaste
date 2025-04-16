using AIPaste.Models.DTO;

namespace AIPaste.Models.SettingsServices
{
    public interface ISettingsService
    {
        IAppSettings LoadSettings();
        void SaveSettings(IAppSettings appSettings);
        IAppSettings ResetSettings();
    }
}
