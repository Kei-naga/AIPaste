using System.Collections.Generic;
using System.Linq;
using Windows.System;

namespace AIPaste.Models.DataModels
{
    public class KeyPattern(HOT_KEY_MODIFIERS modifiers, VirtualKey key) : IKeyPattern
    {
        public HOT_KEY_MODIFIERS Modifiers { get; set; } = modifiers;
        public VirtualKey Key { get; set; } = key;

        public string[] AnalyzeModifier()
        {
            var modifiers = new List<string>();
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT))
            {
                modifiers.Add("Alt");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL))
            {
                modifiers.Add("Ctrl");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT))
            {
                modifiers.Add("Shift");
            }
            if (Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN))
            {
                modifiers.Add("Win");
            }
            return modifiers.ToArray();
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
            return $"{string.Join("+", AnalyzeModifier())}+{Key}";
        }

        public bool Equals(IKeyPattern otherPattern)
        {
            return Modifiers == otherPattern.Modifiers && Key == otherPattern.Key;
        }
    }

    public enum HOT_KEY_MODIFIERS : uint
    {
        MOD_ALT = 0x00000001,
        MOD_CONTROL = 0x00000002,
        MOD_NOREPEAT = 0x00004000,
        MOD_SHIFT = 0x00000004,
        MOD_WIN = 0x00000008,
    }

    public interface IKeyPattern
    {
        HOT_KEY_MODIFIERS Modifiers { get; set; }
        VirtualKey Key { get; set; }
        string[] AnalyzeModifier();
        string ToString();
        abstract bool Equals(IKeyPattern otherPattern);
    }
}
