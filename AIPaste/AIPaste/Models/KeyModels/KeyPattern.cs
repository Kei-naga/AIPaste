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

        public string[] AnalyzeModifier()
        {
            string[] modifiers = [];
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
            {
                modifiers.Append("Alt");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
            {
                modifiers.Append("Ctrl");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
            {
                modifiers.Append("Shift");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN))
            {
                modifiers.Append("Win");
            }
            return modifiers;
        }

        public static HOT_KEY_MODIFIERS GetKeyModifierFromString(string[] modifiers)
        {
            HOT_KEY_MODIFIERS modifier = 0;
            foreach (var mod in modifiers)
            {
                switch (mod)
                {
                    case "Alt":
                        modifier |= HOT_KEY_MODIFIERS.MOD_ALT;
                        break;
                    case "Ctrl":
                        modifier |= HOT_KEY_MODIFIERS.MOD_CONTROL;
                        break;
                    case "Shift":
                        modifier |= HOT_KEY_MODIFIERS.MOD_SHIFT;
                        break;
                    case "Win":
                        modifier |= HOT_KEY_MODIFIERS.MOD_WIN;
                        break;
                }
            }
            return modifier;
        }

        public static HOT_KEY_MODIFIERS GetModifiers(bool ctrl, bool alt, bool shift, bool win)
        {
            HOT_KEY_MODIFIERS modifier = 0;
            if (ctrl)
            {
                modifier |= HOT_KEY_MODIFIERS.MOD_CONTROL;
            }
            if (alt)
            {
                modifier |= HOT_KEY_MODIFIERS.MOD_ALT;
            }
            if (shift)
            {
                modifier |= HOT_KEY_MODIFIERS.MOD_SHIFT;
            }
            if (win)
            {
                modifier |= HOT_KEY_MODIFIERS.MOD_WIN;
            }
            return modifier;
        }

        public override string ToString()
        {
            return $"{AnalyzeModifier()}+{Key}";
        }
    }
}
