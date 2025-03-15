using AIPaste.Models.DataModels;
using NLog;

namespace AIPaste.Models.SettingsServices
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsStore _settingsStore;
        private AppSettings _presentAppSettings;

        static private SettingsService? _instance;

        public static SettingsService GetInstance(ISettingsStore? settingsStore = null)
        {
            settingsStore ??= new SettingsStore();
            if (_instance == null || _instance._settingsStore.GetType != settingsStore.GetType)
            {
                _instance = new SettingsService(settingsStore);
            }
            return _instance;
        }

        private SettingsService(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
            _presentAppSettings = _settingsStore.LoadSettings();
        }

        public AppSettings LoadSettings()
        {
            return _presentAppSettings;
        }

        public void SaveSettings(AppSettings appSettings)
        {
            _presentAppSettings = appSettings;
            _settingsStore.SaveSettings(appSettings);
        }

        public AppSettings ResetSettings()
        {
            _presentAppSettings = _settingsStore.ResetSettings();
            return _presentAppSettings;
        }
    }
}
