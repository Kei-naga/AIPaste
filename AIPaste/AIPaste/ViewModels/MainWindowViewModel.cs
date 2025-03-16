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
        private AppSettings _appSettings;
        private readonly IHotKeyManager _hotKeyManager;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        
        public MainWindowViewModel(Action action, ISettingsService? settingsService = null)
        {
            settingsService ??= SettingsService.GetInstance();
            _appSettings = settingsService.LoadSettings();
            _hotKeyManager = new HotKeyManager(action);
            RegisterHotKeyFirstly(settingsService);
        }

        public MainWindowViewModel(IHotKeyManager hotKeyManager, ISettingsService? settingsService = null)
        {
            settingsService ??= SettingsService.GetInstance();
            _appSettings = settingsService.LoadSettings();
            _hotKeyManager = hotKeyManager;
            RegisterHotKeyFirstly(settingsService);
        }

        // TODO: ここらへんの処理は切り出して別クラスにしたい。ホットキー用のシングルトンクラスを作って、そこに処理を移す
        private void RegisterHotKeyFirstly(ISettingsService settingsService)
        {
            if (_appSettings.KeySettings.IsHotkeyEnabled && !RegisterHotKey())
            {
                var keySettings = new KeySettings(false, _appSettings.KeySettings.KeyPattern);
                _appSettings.KeySettings = keySettings;
                settingsService.SaveSettings(_appSettings);
            }
        }

        public bool UpdateSettings(AppSettings appSettings)
        {
            _appSettings = appSettings;
            if (_appSettings.KeySettings.IsHotkeyEnabled)
            {
                return RegisterHotKey();
            }
            else
            {
                UnRegisterHotKey();
                return true;
            }
        }

        private bool RegisterHotKey()
        {
            _hotKeyManager.UnRegisterHotKey();
            return _hotKeyManager.RegisterHotKey(_appSettings.KeySettings.KeyPattern);
        }

        public void UnRegisterHotKey() => _hotKeyManager.UnRegisterHotKey();
    }
}
