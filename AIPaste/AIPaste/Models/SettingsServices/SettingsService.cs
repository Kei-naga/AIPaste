using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPaste.Models.SettingsServices
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsStore _settingsStore;
        private IAppSettings _presentAppSettings;
        private readonly IMyLogger _logger;

        static private SettingsService? _instance;

        public static SettingsService GetInstance(ISettingsStore? settingsStore = null, IMyLogger? logger = null)
        {
            settingsStore ??= new SettingsStore(logger);
            if (_instance == null || _instance._settingsStore.GetType() != settingsStore.GetType())
            {
                _instance = new SettingsService(settingsStore, logger);
            }
            return _instance;
        }

        private SettingsService(ISettingsStore settingsStore, IMyLogger? logger)
        {
            _logger = logger ?? MyLogger.GetInstance();
            _settingsStore = settingsStore;
            _presentAppSettings = _settingsStore.LoadSettings();
        }

        public IAppSettings LoadSettings()
        {
            return _presentAppSettings;
        }

        public void SaveSettings(IAppSettings appSettings)
        {
            _logger.Info("SAVE_SETTINGS");
            _presentAppSettings = appSettings;
            _settingsStore.SaveSettings(appSettings);
        }

        public IAppSettings ResetSettings()
        {
            _presentAppSettings = _settingsStore.ResetSettings();
            return _presentAppSettings;
        }

        public static void InitializeInstance() => _instance = null;
    }
}
