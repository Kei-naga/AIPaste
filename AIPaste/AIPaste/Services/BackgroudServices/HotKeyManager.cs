using System;
using AIPaste.Models.KeyModels;
using NLog;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AIPaste.Services.BackgroudServices
{
    internal partial class HotKeyManager(Action action) : IDisposable
    {
        private HotkeyMessageDummyWindow? _DummuyWindow;
        public KeyPattern KeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
        private Action _onHotKeyPressed = action;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public bool RegisterHotKey(KeyPattern keyPattern)
        {
            _DummuyWindow?.Dispose();
            KeyPattern = keyPattern;
            _DummuyWindow = new HotkeyMessageDummyWindow(_onHotKeyPressed);
            return _DummuyWindow.RegisterHotKey(KeyPattern);
        }

        public void Dispose()
        {
            _DummuyWindow?.Dispose();
            _DummuyWindow = null;
        }
    }
}
