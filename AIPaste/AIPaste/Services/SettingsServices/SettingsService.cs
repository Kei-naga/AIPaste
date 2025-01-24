using AIPaste.Models.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPaste.Services.SettingsServices
{
    internal class SettingsService
    {
        public SettingsService() { }
        public AppSettings LoadSettings()
        {
            var llmModelSettings = new LLMModelSettings(
                ModelPath: @"C:\Users\keita\llama\Llama-3-ELYZA-JP-8B-GGUF\Llama-3-ELYZA-JP-8B-q4_k_m.gguf",
                GpuLayerCount: 32,
                ContextSize: 1024,
                AntiPrompts: ["END"],
                MaxTokens: 256
            );
            var keySettings = new KeySettings(
                KeyPattern: "Ctrl+Shift+V"
                );
            return new AppSettings(llmModelSettings, true, keySettings);
        }
        public void SaveSettings(AppSettings appSettings)
        {
            return;
        }

    }
}
