using Windows.System;

namespace AIPaste.Models.DataModels
{
    public struct KeySettings(bool IsHotkeyEnabled, IKeyPattern KeyPattern) : IKeySettings
    {
        public bool IsHotkeyEnabled { get; set; } = IsHotkeyEnabled;
        public IKeyPattern KeyPattern { get; set; } = KeyPattern;

        public static IKeySettings GetDefaultSettings()
        {
            var defaultKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
            var defaultIsHotkeyEnabled = true;
            return new KeySettings(defaultIsHotkeyEnabled, defaultKeyPattern);
        }

        public override readonly string ToString()
        {
            return $"IsHotkeyEnabled:{IsHotkeyEnabled}, HotKey:{KeyPattern}";
        }

        public readonly bool Equals(IKeySettings otherSettings)
        {
            return IsHotkeyEnabled == otherSettings.IsHotkeyEnabled && KeyPattern.Equals(otherSettings.KeyPattern);
        }
    }

    public interface IKeySettings
    {
        bool IsHotkeyEnabled { get; set; }
        IKeyPattern KeyPattern { get; set; }
        abstract string ToString();
        abstract bool Equals(IKeySettings otherSettings);
    }
}
