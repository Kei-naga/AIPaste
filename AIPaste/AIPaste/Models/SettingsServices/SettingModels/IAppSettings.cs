
namespace AIPaste.Models.SettingsServices.SettingModels
{
    public interface IAppSettings
    {
        bool AutoStart { get; set; }
        IKeySettings KeySettings { get; set; }
        ILlmModelSettings[] ModelSettingsList { get; set; }
        string ToString();
        bool Equals(IAppSettings otherSettings);
        ILlmModelSettings? GetLlmModelSettings(ModelType modelType);
        IActiveLlmModels ActiveLlmModels { get; set; }
    }
}
