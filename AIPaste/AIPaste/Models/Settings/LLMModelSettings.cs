using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Content;
using Windows.Storage;

namespace AIPaste.Models.Settings
{
    internal struct LLMModelSettings(string ModelPath, int GpuLayerCount, uint ContextSize, List<string> AntiPrompts, int MaxTokens)
    {
        public string ModelPath { get; set; } = ModelPath;
        public int GpuLayerCount { get; set; } = GpuLayerCount;
        public uint ContextSize { get; set; } = ContextSize;
        public List<string> AntiPrompts { get; set; } = AntiPrompts;
        public int MaxTokens { get; set; } = MaxTokens;

        public static LLMModelSettings GetDefaultSettings()
        {
            return new LLMModelSettings(
                ModelPath: @"C:\Users\keita\llama\Llama-3-ELYZA-JP-8B-GGUF\Llama-3-ELYZA-JP-8B-q4_k_m.gguf",
                GpuLayerCount: 32,
                ContextSize: 1024,
                AntiPrompts: ["END"],
                MaxTokens: 256
            );
        }
        public readonly bool Equals(LLMModelSettings otherSettings)
        {
            return ModelPath == otherSettings.ModelPath &&
                GpuLayerCount == otherSettings.GpuLayerCount &&
                ContextSize == otherSettings.ContextSize &&
                AntiPrompts.SequenceEqual(otherSettings.AntiPrompts) &&
                MaxTokens == otherSettings.MaxTokens;
        }

    }
}
