using System;
using AIPaste.Models.Settings;
using AIPaste.Services.BackgroudServices;
using AIPaste.Services.SettingsServices;
using NLog;

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel
    {
        private AppSettings _appSettings;
        private HotKeyHandler? _hotKeyHandler;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Action _action;
        public MainWindowViewModel(Action action)
        {
            var settingsService = new SettingsService();
            _appSettings = settingsService.LoadSettings();
            _action = action;
            // TODO: ここらへんの処理は切り出して別クラスにしたい。ホットキー用のシングルトンクラスを作って、そこに処理を移す
            if (IsHotKeyEnabled() && !RegisterHotKey())
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
            if (IsHotKeyEnabled())
            {
                return RegisterHotKey();
            }
            else
            {
                UnRegisterHotKey();
                return true;
            }
        }

        private bool IsHotKeyEnabled()
        {
            return _appSettings.KeySettings.IsHotkeyEnabled;
        }

        private bool RegisterHotKey()
        {
            if (_hotKeyHandler != null)
            {
                UnRegisterHotKey();
            }
            _hotKeyHandler = new HotKeyHandler(_action);
            return _hotKeyHandler.RegisterHotKey(_appSettings.KeySettings.KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _hotKeyHandler?.Dispose();
            _hotKeyHandler = null;
        }
    }
}
