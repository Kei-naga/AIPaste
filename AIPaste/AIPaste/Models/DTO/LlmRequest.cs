using System;
using AIPaste.common;

namespace AIPaste.Models.DTO
{
    public class LlmRequest(string targetText, string userInput, IResourceLoaderWrapper? resourceLoader = null) : ILlmRequest
    {
        public string TargetText { get; set; } = targetText;
        public string UserInput { get; set; } = userInput;
        private readonly IResourceLoaderWrapper _resourceLoader = resourceLoader ?? new ResourceLoaderWrapper();

        public string ToOptimizedRequest()
        {
            return _resourceLoader.GetString("/LLMResources/TargetTextFlagForOptimizingText") + Environment.NewLine
                + TargetText + Environment.NewLine
                + _resourceLoader.GetString("/LLMResources/UserInstructionFlagForOptimizingText") + Environment.NewLine
                + UserInput;
        }
        public override string ToString() => ToOptimizedRequest();
    }

    public interface ILlmRequest
    {
        string TargetText { get; set; }
        string UserInput { get; set; }
        string ToOptimizedRequest();
    }
}
