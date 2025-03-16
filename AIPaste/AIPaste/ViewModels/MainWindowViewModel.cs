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

        // TODO: ここらへんの処理は切り出して別クラスにしたい。ホットキー用のシングルトンクラスを作って、そこに処理を移す
        private void RegisterHotKeyFirstly(AppSettings appSettings)
        {
            if (!UpdateHotkeySettings(appSettings.KeySettings))
            {
                var keySettings = new KeySettings(false, appSettings.KeySettings.KeyPattern);
                appSettings.KeySettings = keySettings;
                _settingsService.SaveSettings(appSettings);
            }
        }

        public bool UpdateHotkeySettings(KeySettings keySettings)
        {
            if (keySettings.IsHotkeyEnabled)
            {
                return RegisterHotKey(keySettings.KeyPattern);
            }
            else
            {
                UnRegisterHotKey();
                return true;
            }
        }

        private bool RegisterHotKey(KeyPattern keyPattern)
        {
            _hotKeyManager.UnRegisterHotKey();
            return _hotKeyManager.RegisterHotKey(keyPattern);
        }

        public void UnRegisterHotKey() => _hotKeyManager.UnRegisterHotKey();
    }
}
