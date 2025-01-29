using ManagedCuda;

namespace AIPaste.Models.Settings
{
    internal class AppSettings(LLMModelSettings modelSettings, KeySettings keySettings, bool autoStartSetting, bool gpuAvailable)
    {
        public LLMModelSettings LLMModelSettings { get; set; } = modelSettings;
        public bool GpuEnabled { get; set; } = gpuAvailable;
        public bool AutoStart { get; set; } = autoStartSetting;
        public KeySettings KeySettings { get; set; } = keySettings;

        public static AppSettings GetDefaultSettings()
        {
            var llmModelSettings = LLMModelSettings.GetDefaultSettings();
            var keySettings = KeySettings.GetDefaultSettings();
            return new AppSettings(
                modelSettings: llmModelSettings,
                keySettings: keySettings,
                autoStartSetting: true,
                gpuAvailable: IsGpuAvailable()
            );
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

        public override string ToString()
        {
            return $"ModelSettings: {LLMModelSettings}, KeySettings: {KeySettings}, AutoStart: {AutoStart}, GpuEnabled: {GpuEnabled}";
        }
    }
}



