using System;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using NLog;
using Windows.System;

namespace AIPaste.Services.BackgroudServices
{
    public partial class HotKeyManager : IHotKeyManager
    {
        private HotkeyMessageManager? _hotkeyMessageManager;
        public IKeyPattern KeyPattern { get; set; } = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
        private readonly Action _onHotKeyPressed;
        private static HotKeyManager? _instance;

        private HotKeyManager(Action action)
        {
            _onHotKeyPressed = action;
        }

        /// <summary>
        /// Create a singleton instance of HotKeyManager.
        /// </summary>
        public static HotKeyManager GetInstance(Action action)
        {
            if (_instance == null)
            {
                _instance = new HotKeyManager(action);
            }
            return _instance;
        }

        /// <summary>
        /// Get the singleton instance of HotKeyManager.
        /// </summary>
        /// <remarks>
        /// If the instance is not created yet, it will return null.
        /// Create the instance using GetInstance(Action action) first.
        /// </remarks>
        public static HotKeyManager? GetInstance()
        {
            return _instance;
        }

        public bool RegisterHotKey(IKeyPattern keyPattern)
        {
            _hotkeyMessageManager?.Dispose();
            KeyPattern = keyPattern;
            _hotkeyMessageManager = new HotkeyMessageManager(_onHotKeyPressed);
            return _hotkeyMessageManager.RegisterHotKey(KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _hotkeyMessageManager?.Dispose();
            _hotkeyMessageManager = null;
        }
    }
}
