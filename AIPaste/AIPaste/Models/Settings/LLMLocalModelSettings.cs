using ManagedCuda;

namespace AIPaste.Models.Settings
{
    internal class LLMLocalModelSettings(string ModelPath, bool GpuEnable, int GpuLayerCount, uint ContextSize, int MaxTokens): ILLMModelSettings
    {
        public string ModelPath { get; set; } = ModelPath;
        public int GpuLayerCount { get; set; } = GpuLayerCount;
        public uint ContextSize { get; set; } = ContextSize;
        public int MaxTokens { get; set; } = MaxTokens;
        public bool GpuEnabled { get; set; } = GpuEnable;

        public static ILLMModelSettings GetDefaultSettings()
        {
            return new LLMLocalModelSettings(
                ModelPath: @"",
                GpuEnable: IsGpuAvailable(),
                GpuLayerCount: 32,
                ContextSize: 1024,
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
                ContextSize == otherLocalSettings.ContextSize &&
                MaxTokens == otherLocalSettings.MaxTokens;
        }

        public override string ToString()
        {
            return $"ModelPath: {ModelPath}, GpuLayerCount: {GpuLayerCount}, ContextSize: {ContextSize}, MaxTokens: {MaxTokens}";
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
