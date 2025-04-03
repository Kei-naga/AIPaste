using System.Linq;

namespace AIPaste.Models.DataModels
{
    public class AppSettings(bool autoStartSetting, ModelType modelType, IKeySettings keySettings, ILLMModelSettings[] modelSettingsList) : IAppSettings
    {
        public bool AutoStart { get; set; } = autoStartSetting;
        public ModelType ModelType { get; set; } = modelType;
        public IKeySettings KeySettings { get; set; } = keySettings;
        public ILLMModelSettings[] ModelSettingsList { get; set; } = modelSettingsList;

        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings(
                autoStartSetting: true,
                modelType: ModelType.LocalLLM,
                keySettings: DataModels.KeySettings.GetDefaultSettings(),
                modelSettingsList: new ILLMModelSettings[] { LLMLocalModelSettings.GetDefaultSettings(), GeminiModelSettings.GetDefaultSettings() }
            );
        }

        public override string ToString()
        {
            return $"ModelSettings:  AutoStart: {AutoStart}, ModelType: {ModelType}";
        }

        public bool Equals(IAppSettings otherSettings)
        {
            return AutoStart == otherSettings.AutoStart &&
                ModelType == otherSettings.ModelType &&
                KeySettings.Equals(otherSettings.KeySettings) &&
                SameLlmSettings(otherSettings.ModelSettingsList);
        }

        private bool SameLlmSettings(ILLMModelSettings[] otherLlmSettingsList)
        {
            return ModelSettingsList.Select(x =>
            {
                var otherLlmSettings = otherLlmSettingsList.FirstOrDefault(y => y.GetType() == x.GetType());
                return otherLlmSettings != null && x.Equals(otherLlmSettings);
            }).All(x => x);
        }
    }

    public enum ModelType
    {
        LocalLLM,
        Gemini,
    }
}



