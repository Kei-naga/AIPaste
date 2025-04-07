using System;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;
using NLog;

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IHotKeyManager _hotKeyManager;
        private readonly ISettingsService _settingsService;
        private readonly ILogger _logger;

        public MainWindowViewModel(
            Action action, 
            IHotKeyManagerFactory? hotKeyManagerFactory = null, 
            ISettingsService? settingsService = null, 
            ILogger? logger = null )
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();
            _logger.Trace("MainWindowViewModel created");
            hotKeyManagerFactory ??= new HotkeyManagerFactory();
            _hotKeyManager = hotKeyManagerFactory.CreateHotKeyManager(action);
            _settingsService = settingsService ?? SettingsService.GetInstance();
            var appSettings = _settingsService.LoadSettings();
            RegisterHotKeyFirstly(appSettings);
        }

        private void RegisterHotKeyFirstly(IAppSettings appSettings)
        {
            try
            {
                _hotKeyManager.UpdateHotkeySettings(appSettings.KeySettings);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to register hotkey");
                _hotKeyManager.UnRegisterHotKey();
                var keySettings = new KeySettings(false, appSettings.KeySettings.KeyPattern);
                appSettings.KeySettings = keySettings;
                _settingsService.SaveSettings(appSettings);
            }
        }

        public void UnRegisterHotKey() => _hotKeyManager.UnRegisterHotKey();
    }
}
