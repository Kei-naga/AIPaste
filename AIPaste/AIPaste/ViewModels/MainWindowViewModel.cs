using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.Settings;
using AIPaste.Services.BackgroudServices;
using AIPaste.Services.SettingsServices;
using NLog;

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel
    {
        private AppSettings _appSettings { get; set; }
        private HotKeyHandler? _hotKeyHandler;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Action _action;
        public MainWindowViewModel(Action action)
        {
            var settingsService = new SettingsService();
            _appSettings = settingsService.LoadSettings();
            _action = action;
            if (IsHotKeyEnabled())
            {
                RegisterHotKey();
            }
        }

        internal void UpdateSettings(AppSettings appSettings)
        {
            _appSettings = appSettings;
            if (IsHotKeyEnabled())
            {
                RegisterHotKey();
            }
            else
            {
                UnRegisterHotKey();
            }
        }

        private bool IsHotKeyEnabled(){
            return _appSettings.KeySettings.IsHotkeyEnabled;
        }

        private void RegisterHotKey()
        {
            if (_hotKeyHandler != null)
            {
                UnRegisterHotKey();
            }
            _hotKeyHandler = new HotKeyHandler(_action);
            _hotKeyHandler.RegisterHotKey(KeySettings.GetDefaultSettings().KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _hotKeyHandler?.Dispose();
            _hotKeyHandler = null;
        }
    }
}
