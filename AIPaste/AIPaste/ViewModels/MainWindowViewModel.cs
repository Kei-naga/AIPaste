using System;
using AIPaste.common;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IHotKeyManager _hotKeyManager;
        private readonly ISettingsService _settingsService;
        private readonly IMyLogger _logger;

        public MainWindowViewModel(
            Action action, 
            IHotKeyManagerFactory? hotKeyManagerFactory = null, 
            ISettingsService? settingsService = null, 
            IMyLogger? logger = null )
        {
            _logger = logger ?? MyLogger.GetInstance();
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
                _logger.Error("FAILED_REGISTER_HOTKEY");
                _logger.Debug(ex);
                _hotKeyManager.UnRegisterHotKey();
                var keySettings = new KeySettings(false, appSettings.KeySettings.KeyPattern);
                appSettings.KeySettings = keySettings;
                _settingsService.SaveSettings(appSettings);
            }
        }

        public void UnRegisterHotKey() => _hotKeyManager.UnRegisterHotKey();
    }
}
