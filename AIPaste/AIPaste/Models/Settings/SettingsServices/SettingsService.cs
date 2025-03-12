using NLog;

namespace AIPaste.Models.Settings.SettingsServices
{
    internal class SettingsService : ISettingsService
    {
        private readonly SettingsStore _settingsStore;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private AppSettings _presentAppSettings;

        static private SettingsService? _instance;
        public static SettingsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SettingsService();
                }
                return _instance;
            }
        }

        private SettingsService()
        {
            _settingsStore = new SettingsStore();
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
