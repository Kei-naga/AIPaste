using System;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using NLog;
using Windows.System;

namespace AIPaste.Services.BackgroudServices
{
    public partial class HotKeyManager(Action action) : IHotKeyManager
    {
        private HotkeyMessageManager? _hotkeyMessageManager;
        public KeyPattern KeyPattern { get; set; } = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
        private readonly Action _onHotKeyPressed = action;

        public bool RegisterHotKey(KeyPattern keyPattern, IHotkeyControler? hotkeyControler = null)
        {
            hotkeyControler ??= new SystemHotkeyControler();
            _hotkeyMessageManager?.Dispose();
            KeyPattern = keyPattern;
            _hotkeyMessageManager = new HotkeyMessageManager(_onHotKeyPressed, hotkeyControler);
            return _hotkeyMessageManager.RegisterHotKey(KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _hotkeyMessageManager?.Dispose();
            _hotkeyMessageManager = null;
        }
    }
}
