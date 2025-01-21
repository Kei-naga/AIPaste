using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCuda;

namespace AIPaste.Models.Settings
{
    internal class AppSettings
    {
        public LLMModelSettings LLMModelSettings { get; set; }
        public bool GpuEnabled { get; set; }
        public bool AutoStart { get; set; }
        public KeySettings KeySettings { get; set; }

        public AppSettings(LLMModelSettings modelSettings, bool autoStartSetting, KeySettings keySettings)
        {
            LLMModelSettings = modelSettings;
            GpuEnabled = IsGpuAvailable();
            AutoStart = autoStartSetting;
            KeySettings = keySettings;
        }

        private bool IsGpuAvailable() // cheking for only nvidia gpu
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
