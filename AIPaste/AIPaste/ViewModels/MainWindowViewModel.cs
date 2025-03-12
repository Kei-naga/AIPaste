using System;
using AIPaste.Models.Settings;
using AIPaste.Models.Settings.SettingsServices;
using AIPaste.Services.BackgroudServices;
using Microsoft.UI.Xaml;
using NLog;

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel
    {
        private AppSettings _appSettings;
        private HotKeyManager? _hotKeyManager;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Action _action;
        public MainWindowViewModel(Action action)
        {
            var settingsService = SettingsService.Instance;
            _appSettings = settingsService.LoadSettings();
            _action = action;
            // TODO: ここらへんの処理は切り出して別クラスにしたい。ホットキー用のシングルトンクラスを作って、そこに処理を移す
            if (_appSettings.KeySettings.IsHotkeyEnabled && !RegisterHotKey())
            {
                var keySettings = _appSettings.KeySettings;
                keySettings.IsHotkeyEnabled = false;
                _appSettings.KeySettings = keySettings;
                settingsService.SaveSettings(_appSettings);
            }
        }

        internal bool UpdateSettings(AppSettings appSettings)
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
            if (_hotKeyManager != null)
            {
                UnRegisterHotKey();
            }
            _hotKeyManager = new HotKeyManager(_action);
            return _hotKeyManager.RegisterHotKey(_appSettings.KeySettings.KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _hotKeyManager?.Dispose();
            _hotKeyManager = null;
        }
    }
}
