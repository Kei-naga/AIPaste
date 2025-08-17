namespace AIPaste.DTO.SettingsDTO
{
    public class EnabledModelDTO(bool IsLocalLlmActive, bool IsGeminiActive)
    {
        public bool IsLocalLlmActive { get; } = IsLocalLlmActive;
        public bool IsGeminiActive { get; } = IsGeminiActive;
    }
}
