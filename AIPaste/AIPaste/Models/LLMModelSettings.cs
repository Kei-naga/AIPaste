using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Content;

namespace AIPaste.Models
{
    internal struct LLMModelSettings(string ModelPath, int GpuLayerCount, uint ContextSize, string[] antiPrompts, int MaxTokens)
    {
        public string ModelPath { get; set; } = ModelPath;
        public int GpuLayerCount { get; set; } = GpuLayerCount;
        public uint ContextSize { get; set; } = ContextSize;
        public string[] antiPrompts { get; set; } = antiPrompts;
        public int MaxTokens { get; set; } = MaxTokens;
    }
}
