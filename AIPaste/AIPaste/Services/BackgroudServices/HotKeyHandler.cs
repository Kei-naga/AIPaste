using System;
using AIPaste.Models.KeyModels;
using NLog;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AIPaste.Services.BackgroudServices
{
    internal partial class HotKeyHandler : IDisposable
    {
        private HotkeyMessageDummyWindow? _DummuyWindow;
        public KeyPattern KeyPattern;
        private Action _onHotKeyPressed;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public HotKeyHandler(Action action)
        {
            _onHotKeyPressed = action;
            KeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C); // Default hotkey
        }

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
