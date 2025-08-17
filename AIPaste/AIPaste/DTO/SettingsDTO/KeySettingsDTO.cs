namespace AIPaste.DTO.SettingsDTO
{
    public class KeySettingsDTO(bool IsHotkeyEnabled, KeyPatternDTO KeyPattern)
    {
        public bool IsHotkeyEnabled { get; } = IsHotkeyEnabled;
        public KeyPatternDTO KeyPattern { get; } = KeyPattern;
    }
}
