namespace AIPaste.Models.SettingsServices.SettingModels
{
    public class LlmLocalModelSettings(string ModelPath, bool GpuEnable, int GpuLayerCount, uint MaxContextSize, int MaxTokens) : ILlmModelSettings
    {
        public string ModelPath { get; set; } = ModelPath;
        public int GpuLayerCount { get; set; } = GpuLayerCount;
        public uint MaxContextSize { get; set; } = MaxContextSize;
        public int MaxTokens { get; set; } = MaxTokens;
        public bool GpuEnabled { get; set; } = GpuEnable;

        public static ILlmModelSettings GetDefaultSettings()
        {
            return new LlmLocalModelSettings(
                ModelPath: @"",
                GpuEnable: true,
                GpuLayerCount: 32,
                MaxContextSize: 1024,
                MaxTokens: 256
            );
        }
        public bool Equals(ILlmModelSettings otherSettings)
        {
            if (otherSettings is not LlmLocalModelSettings)
            {
                return false;
            }
            var otherLocalSettings = (LlmLocalModelSettings)otherSettings;
            return ModelPath == otherLocalSettings.ModelPath &&
                GpuLayerCount == otherLocalSettings.GpuLayerCount &&
                MaxContextSize == otherLocalSettings.MaxContextSize &&
                MaxTokens == otherLocalSettings.MaxTokens;
        }

        public override string ToString()
        {
            return $"ModelPath: {ModelPath}, GpuLayerCount: {GpuLayerCount}, MaxContextSize: {MaxContextSize}, MaxTokens: {MaxTokens}";
        }
    }
}
