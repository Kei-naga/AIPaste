using System.Linq;

namespace AIPaste.Models.SettingsServices.SettingModels
{
    public class AppSettings(bool autoStartSetting, IKeySettings keySettings, ILlmModelSettings[] modelSettingsList, IActiveLlmModels activeLlmModels) : IAppSettings
    {
        public bool AutoStart { get; set; } = autoStartSetting;
        public IKeySettings KeySettings { get; set; } = keySettings;
        public ILlmModelSettings[] ModelSettingsList { get; set; } = modelSettingsList;
        public IActiveLlmModels ActiveLlmModels { get; set; } = activeLlmModels;

        public static AppSettings GetDefaultSettings()
        {
            return new AppSettings(
                autoStartSetting: true,
                keySettings: SettingModels.KeySettings.GetDefaultSettings(),
                modelSettingsList: [LlmLocalModelSettings.GetDefaultSettings(), GeminiModelSettings.GetDefaultSettings()],
                activeLlmModels: SettingModels.ActiveLlmModels.GetDefaultSettings()
            );
        }

        public override string ToString()
        {
            return $"KeySettings: [{KeySettings}],  AutoStart: {AutoStart}, IsLocalLlmActive: [{ActiveLlmModels}]";
        }

        public bool Equals(IAppSettings otherSettings)
        {
            return AutoStart == otherSettings.AutoStart &&
                KeySettings.Equals(otherSettings.KeySettings) &&
                SameLlmSettings(otherSettings.ModelSettingsList) &&
                ActiveLlmModels.Equals(otherSettings.ActiveLlmModels);
        }

        private bool SameLlmSettings(ILlmModelSettings[] otherLlmSettingsList)
        {
            return ModelSettingsList.Select(x =>
            {
                var otherLlmSettings = otherLlmSettingsList.FirstOrDefault(y => y.GetType() == x.GetType());
                return otherLlmSettings != null && x.Equals(otherLlmSettings);
            }).All(x => x);
        }

        public ILlmModelSettings? GetLlmModelSettings(ModelType modelType)
        {
            return modelType switch
            {
                ModelType.LocalLLM => ModelSettingsList.FirstOrDefault(x => x is LlmLocalModelSettings) ?? null,
                ModelType.Gemini => ModelSettingsList.FirstOrDefault(x => x is GeminiModelSettings) ?? null,
                _ => null,
            };
        }
    }

    public enum ModelType
    {
        LocalLLM,
        Gemini,
    }
}



