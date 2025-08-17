namespace AIPaste.DTO.SettingsDTO
{
    public class AppSettingsDTO(bool autoStartSetting, KeySettingsDTO keySettings, EnabledModelDTO enabledModel, LocalModelSettingsDTO? localModelSettings, GeminiSettingsDTO? geminiSettings)
    {
        public bool AutoStart { get; set; } = autoStartSetting;
        public KeySettingsDTO KeySettings { get; } = keySettings;
        public EnabledModelDTO ActiveLlmModels { get; } = enabledModel;
        public LocalModelSettingsDTO? LocalModelSettings { get; } = localModelSettings;
        public GeminiSettingsDTO? GeminiSettings { get; } = geminiSettings;
    }
}
