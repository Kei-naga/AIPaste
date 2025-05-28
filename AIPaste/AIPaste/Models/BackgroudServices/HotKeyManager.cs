using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;
using Windows.System;

namespace AIPaste.Models.BackgroudServices
{
    public partial class HotKeyManager : IHotKeyManager
    {
        private IHotkeyMessageManager _hotkeyMessageManager;
        public IKeyPattern KeyPattern { get; set; } = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
        private static HotKeyManager? _instance;
        private readonly IMyLogger _logger;

        private HotKeyManager(IHotkeyMessageManager hotkeyMessageManager, IMyLogger logger)
        {
            _hotkeyMessageManager = hotkeyMessageManager;
            _logger = logger;
        }

        /// <summary>
        /// Create a singleton instance of HotKeyManager.
        /// </summary>
        public static HotKeyManager CreateInstance(IHotkeyMessageManager hotkeyMessageManager, IMyLogger logger)
        {
            _instance = new HotKeyManager(hotkeyMessageManager, logger);
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

        public static void DisposeInstance()
        {
            _instance = null;
        }

        public void RegisterHotKey(IKeyPattern keyPattern)
        {
            _logger.Info("REGISTER_HOTKEY");
            KeyPattern = keyPattern;
            _hotkeyMessageManager.RegisterHotKey(KeyPattern);
        }

        public void UnRegisterHotKey()
        {
            _logger.Info("UNREGISTER_HOTKEY");
            _hotkeyMessageManager.UnregisterHotKey();
        }

        public void UpdateHotkeySettings(IKeySettings keySettings)
        {
            if (keySettings.IsHotkeyEnabled)
            {
                RegisterHotKey(keySettings.KeyPattern);
            }
            else
            {
                UnRegisterHotKey();
            }
        }
    }
}
