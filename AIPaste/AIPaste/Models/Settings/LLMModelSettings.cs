using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Content;

namespace AIPaste.Models.Settings
{
    internal struct LLMModelSettings(string ModelPath, int GpuLayerCount, uint ContextSize, string[] AntiPrompts, int MaxTokens)
    {
        public string ModelPath { get; set; } = ModelPath;
        public int GpuLayerCount { get; set; } = GpuLayerCount;
        public uint ContextSize { get; set; } = ContextSize;
        public string[] AntiPrompts { get; set; } = AntiPrompts;
        public int MaxTokens { get; set; } = MaxTokens;
    }
}
