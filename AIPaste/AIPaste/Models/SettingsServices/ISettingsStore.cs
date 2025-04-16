using AIPaste.Models.DTO;

namespace AIPaste.Models.SettingsServices
{
    public interface ISettingsStore
    {
        IAppSettings LoadSettings();
        void SaveSettings(IAppSettings appSettings);
        IAppSettings ResetSettings();
    }
}
