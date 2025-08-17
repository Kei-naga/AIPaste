namespace AIPaste.DTO.SettingsDTO
{
    public class LocalModelSettingsDTO(string ModelPath, bool GpuEnable, int GpuLayerCount, uint MaxContextSize, int MaxTokens)
    {
        public string ModelPath { get; } = ModelPath;
        public int GpuLayerCount { get; } = GpuLayerCount;
        public uint MaxContextSize { get; } = MaxContextSize;
        public int MaxTokens { get; } = MaxTokens;
        public bool GpuEnabled { get; } = GpuEnable;
    }
}
