using ManagedCuda;

namespace AIPaste.Models.DataModels
{
    internal class LLMLocalModelSettings(string ModelPath, bool GpuEnable, int GpuLayerCount, uint MaxContextSize, int MaxTokens) : ILLMModelSettings
    {
        public string ModelPath { get; set; } = ModelPath;
        public int GpuLayerCount { get; set; } = GpuLayerCount;
        public uint MaxContextSize { get; set; } = MaxContextSize;
        public int MaxTokens { get; set; } = MaxTokens;
        public bool GpuEnabled { get; set; } = GpuEnable;

        public static ILLMModelSettings GetDefaultSettings()
        {
            return new LLMLocalModelSettings(
                ModelPath: @"",
                GpuEnable: IsGpuAvailable(),
                GpuLayerCount: 32,
                MaxContextSize: 1024,
                MaxTokens: 256
            );
        }
        public bool Equals(ILLMModelSettings otherSettings)
        {
            if (otherSettings is not LLMLocalModelSettings)
            {
                return false;
            }
            var otherLocalSettings = (LLMLocalModelSettings)otherSettings;
            return ModelPath == otherLocalSettings.ModelPath &&
                GpuLayerCount == otherLocalSettings.GpuLayerCount &&
                MaxContextSize == otherLocalSettings.MaxContextSize &&
                MaxTokens == otherLocalSettings.MaxTokens;
        }

        public override string ToString()
        {
            return $"ModelPath: {ModelPath}, GpuLayerCount: {GpuLayerCount}, MaxContextSize: {MaxContextSize}, MaxTokens: {MaxTokens}";
        }

        private static bool IsGpuAvailable() // cheking for only nvidia gpu
        {
            try
            {
                using var cudaContext = new CudaContext();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
