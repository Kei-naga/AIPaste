namespace AIPaste.Models.DataModels
{
    public class AppSettings(bool autoStartSetting, ModelType modelType, IKeySettings keySettings, LLMLocalModelSettings llmLocalModelSettings, GeminiModelSettings geminiModelSettings): IAppSettings
    {
        public bool AutoStart { get; set; } = autoStartSetting;
        public ModelType ModelType { get; set; } = modelType;
        public IKeySettings KeySettings { get; set; } = keySettings;
        public LLMLocalModelSettings LocalLLMSettings { get; set; } = llmLocalModelSettings;
        public GeminiModelSettings GeminiSettings { get; set; } = geminiModelSettings;

        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings(
                autoStartSetting: true,
                modelType: ModelType.LocalLLM,
                keySettings: DataModels.KeySettings.GetDefaultSettings(),
                llmLocalModelSettings: (LLMLocalModelSettings)LLMLocalModelSettings.GetDefaultSettings(),
                geminiModelSettings: (GeminiModelSettings)GeminiModelSettings.GetDefaultSettings()
            );
        }

        public override string ToString()
        {
            return $"ModelSettings:  AutoStart: {AutoStart}, ModelType: {ModelType}";
        }

        public bool Equals(AppSettings otherSettings)
        {
            return AutoStart == otherSettings.AutoStart &&
                ModelType == otherSettings.ModelType &&
                KeySettings.Equals(otherSettings.KeySettings) &&
                LocalLLMSettings.Equals(otherSettings.LocalLLMSettings) &&
                GeminiSettings.Equals(otherSettings.GeminiSettings);
        }
    }

    public enum ModelType
    {
        LocalLLM,
        Gemini,
    }
}



