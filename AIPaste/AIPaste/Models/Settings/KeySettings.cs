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
    internal struct KeySettings(KeyPattern KeyPattern)
    {
        public KeyPattern KeyPattern = KeyPattern;

        public static KeySettings GetDefaultSettings()
        {
            var defaultKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);
            return new KeySettings(defaultKeyPattern);
        }

        public override string ToString()
        {
            return KeyPattern.ToString();
        }
    }
}
