using AIPaste.common;
using Windows.System;

namespace AIPaste.DTO.SettingsDTO
{
    public class KeyPatternDTO(HOT_KEY_MODIFIERS modifiers, VirtualKey key)
    {
        public HOT_KEY_MODIFIERS Modifiers { get; } = modifiers;
        public VirtualKey Key { get; } = key;
    }
}
