using System;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using NLog;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AIPaste.Services.BackgroudServices
{
    internal partial class HotKeyManager(Action action) : IDisposable, IHotKeyManager
    {
        private HotkeyMessageDummyWindow? _DummuyWindow;
        public KeyPattern KeyPattern { get; set; } = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
        private Action _onHotKeyPressed = action;

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
