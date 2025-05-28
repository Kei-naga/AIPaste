
namespace AIPaste.Models.SettingsServices.SettingModels
{
    public class ActiveLlmModels(bool IsLocalLlmActive, bool IsGeminiActive): IActiveLlmModels
    {
        public bool IsLocalLlmActive { get; set; } = IsLocalLlmActive;
        public bool IsGeminiActive { get; set; } = IsGeminiActive;
        public static ActiveLlmModels GetDefaultSettings()
        {
            return new ActiveLlmModels(true, false);
        }
        public override string ToString()
        {
            return $"IsLocalLlmActive: {IsLocalLlmActive}, IsGeminiActive: {IsGeminiActive}";
        }

        public bool Equals(IActiveLlmModels otherSettings)
        {
            return IsLocalLlmActive == otherSettings.IsLocalLlmActive && IsGeminiActive == otherSettings.IsGeminiActive;
        }
    }

    public interface IActiveLlmModels
    {
        bool IsLocalLlmActive { get; set; }
        bool IsGeminiActive { get; set; }
        string ToString();
        bool Equals(IActiveLlmModels otherSettings);
    }
}
