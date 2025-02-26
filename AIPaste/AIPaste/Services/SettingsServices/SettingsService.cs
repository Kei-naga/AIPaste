using AIPaste.Models.KeyModels;
using AIPaste.Models.Settings;
using NLog;
using System;
using Windows.Storage;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.System;
using AIPaste.Services.LLMServices;
using AIPaste.Services.StartupServices;

namespace AIPaste.Services.SettingsServices
{
    internal class SettingsService
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

        private SettingsService() {
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
