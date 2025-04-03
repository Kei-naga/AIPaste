using System;
using AIPaste.Models.DataModels;
using AIPaste.Models.LLMModels;
using AIPaste.Models.StartupServices;
using NLog;

namespace AIPaste.Models.SettingsServices
{
    public class SettingsService : ISettingsService
    {
        private readonly ISettingsStore _settingsStore;
        private IAppSettings _presentAppSettings;
        private readonly ILogger _logger;

        static private SettingsService? _instance;

        public static SettingsService GetInstance(ISettingsStore? settingsStore = null, ILogger? logger = null)
        {
            settingsStore ??= new SettingsStore(logger);
            if (_instance == null || _instance._settingsStore.GetType != settingsStore.GetType)
            {
                _instance = new SettingsService(settingsStore, logger);
            }
            return _instance;
        }

        private SettingsService(ISettingsStore settingsStore, ILogger? logger)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();
            _settingsStore = settingsStore;
            _presentAppSettings = _settingsStore.LoadSettings();
        }

        public IAppSettings LoadSettings()
        {
            return _presentAppSettings;
        }

        public void SaveSettings(IAppSettings appSettings)
        {
            _presentAppSettings = appSettings;
            _settingsStore.SaveSettings(appSettings);
        }

        public IAppSettings ResetSettings()
        {
            _presentAppSettings = _settingsStore.ResetSettings();
            return _presentAppSettings;
        }
    }
}
