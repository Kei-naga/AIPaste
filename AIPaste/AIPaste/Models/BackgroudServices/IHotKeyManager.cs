using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPaste.Models.BackgroudServices
{
    public interface IHotKeyManager
    {
        IKeyPattern KeyPattern { get; set; }
        void RegisterHotKey(IKeyPattern keyPattern);
        void UnRegisterHotKey();
        void UpdateHotkeySettings(IKeySettings keySettings);
    }
}
