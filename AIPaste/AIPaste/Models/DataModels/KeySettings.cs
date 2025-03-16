using Windows.System;

namespace AIPaste.Models.DataModels
{
    public struct KeySettings(bool IsHotkeyEnabled, KeyPattern KeyPattern)
    {
        public bool IsHotkeyEnabled = IsHotkeyEnabled;
        public KeyPattern KeyPattern = KeyPattern;

        public static KeySettings GetDefaultSettings()
        {
            var defaultKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
            var defaultIsHotkeyEnabled = true;
            return new KeySettings(defaultIsHotkeyEnabled, defaultKeyPattern);
        }

        public override readonly string ToString()
        {
            return $"IsHotkeyEnabled:{IsHotkeyEnabled}, HotKey:{KeyPattern}";
        }

        public readonly bool Equals(KeySettings otherSettings)
        {
            return IsHotkeyEnabled == otherSettings.IsHotkeyEnabled && KeyPattern.Equals(otherSettings.KeyPattern);
        }
    }
}
