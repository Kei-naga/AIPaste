using System.Linq;

namespace AIPaste.Models.DTO
{
    public class AppSettings(bool autoStartSetting, ModelType modelType, IKeySettings keySettings, ILlmModelSettings[] modelSettingsList) : IAppSettings
    {
        public bool AutoStart { get; set; } = autoStartSetting;
        public ModelType ActiveModelType { get; set; } = modelType;
        public IKeySettings KeySettings { get; set; } = keySettings;
        public ILlmModelSettings[] ModelSettingsList { get; set; } = modelSettingsList;

        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings(
                autoStartSetting: true,
                modelType: ModelType.LocalLLM,
                keySettings: DTO.KeySettings.GetDefaultSettings(),
                modelSettingsList: [LlmLocalModelSettings.GetDefaultSettings(), GeminiModelSettings.GetDefaultSettings()]
            );
        }

        public override string ToString()
        {
            return $"ModelSettings:  AutoStart: {AutoStart}, ModelType: {ActiveModelType}";
        }

        public bool Equals(IAppSettings otherSettings)
        {
            return AutoStart == otherSettings.AutoStart &&
                ActiveModelType == otherSettings.ActiveModelType &&
                KeySettings.Equals(otherSettings.KeySettings) &&
                SameLlmSettings(otherSettings.ModelSettingsList);
        }

        private bool SameLlmSettings(ILlmModelSettings[] otherLlmSettingsList)
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



