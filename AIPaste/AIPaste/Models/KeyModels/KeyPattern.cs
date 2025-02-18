using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.System;
using Windows.Globalization;

namespace AIPaste.Models.KeyModels
{
    internal class KeyPattern(HOT_KEY_MODIFIERS modifiers, VirtualKey key)
    {
        public HOT_KEY_MODIFIERS Modifiers { get; set; } = modifiers;
        public VirtualKey Key { get; set; } = key;

        private string AnalyzeModifier()
        {
            StringBuilder sb = new StringBuilder();
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
            {
                sb.Append("Alt+");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
            {
                sb.Append("Ctrl+");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
            {
                sb.Append("Shift+");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN))
            {
                sb.Append("Win+");
            }
            return sb.ToString();
        }

        public override string ToString()
        {
            return $"{AnalyzeModifier()}+{Key}";
        }
    }
}
