using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.KeyModels;
using Windows.Storage;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AIPaste.Models.Settings
{
    internal struct KeySettings(bool IsHotkeyEnabled, KeyPattern KeyPattern)
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
