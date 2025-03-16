using System;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using NLog;
using Windows.System;

namespace AIPaste.Services.BackgroudServices
{
    public partial class HotKeyManager(Action action) : IHotKeyManager
    {
        private HotkeyMessageDummyWindow? _DummuyWindow;
        public KeyPattern KeyPattern { get; set; } = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
        private readonly Action _onHotKeyPressed = action;

        public bool RegisterHotKey(KeyPattern keyPattern, IHotkeyControler? hotkeyControler = null)
        {
            hotkeyControler ??= new SystemHotkeyControler();
            _DummuyWindow?.Dispose();
            KeyPattern = keyPattern;
            _DummuyWindow = new HotkeyMessageDummyWindow(_onHotKeyPressed, hotkeyControler);
            return _DummuyWindow.RegisterHotKey(KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _DummuyWindow?.Dispose();
            _DummuyWindow = null;
        }
    }
}
