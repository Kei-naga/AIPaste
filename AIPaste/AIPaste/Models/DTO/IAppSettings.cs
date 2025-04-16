namespace AIPaste.Models.DTO
{
    public interface IAppSettings
    {
        bool AutoStart { get; set; }
        ModelType ActiveModelType { get; set; }
        IKeySettings KeySettings { get; set; }
        ILlmModelSettings[] ModelSettingsList { get; set; }
        string ToString();
        bool Equals(IAppSettings otherSettings);
    }
}
