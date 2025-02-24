using System;
using ManagedCuda;

namespace AIPaste.Models.Settings
{
    internal class AppSettings(bool autoStartSetting, ModelType modelType, KeySettings keySettings, LLMLocalModelSettings llmLocalModelSettings, GeminiModelSettings geminiModelSettings)
    {
        public bool AutoStart { get; set; } = autoStartSetting;
        public ModelType ModelType { get; set; } = modelType;
        public KeySettings KeySettings { get; set; } = keySettings;
        public LLMLocalModelSettings LocalLLMSettings { get; set; } = llmLocalModelSettings;
        public GeminiModelSettings GeminiSettings { get; set; } = geminiModelSettings;

        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings(
                autoStartSetting: true,
                modelType: ModelType.LocalLLM,
                keySettings: (KeySettings)KeySettings.GetDefaultSettings(),
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



