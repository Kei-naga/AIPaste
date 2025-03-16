using System;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;
using AIPaste.Services.BackgroudServices;
using NLog;
using static System.Collections.Specialized.BitVector32;

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IHotKeyManager _hotKeyManager;
        private readonly ISettingsService _settingsService;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        public MainWindowViewModel(Action action, ISettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? SettingsService.GetInstance();
            var appSettings = _settingsService.LoadSettings();
            _hotKeyManager = new HotKeyManager(action);
            RegisterHotKeyFirstly(appSettings);
        }

        public MainWindowViewModel(IHotKeyManager hotKeyManager, ISettingsService? settingsService = null)
        {
            _settingsService = settingsService ?? SettingsService.GetInstance();
            var appSettings = _settingsService.LoadSettings();
            _hotKeyManager = hotKeyManager;
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
