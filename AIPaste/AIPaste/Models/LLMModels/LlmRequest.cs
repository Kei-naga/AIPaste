using System;
using Windows.ApplicationModel.Resources;

namespace AIPaste.Models.LLMModels
{
    internal class LlmRequest(string targetText, string userInput, ResourceLoader? resourceLoader = null)
    {
        public string TargetText { get; set; } = targetText;
        public string UserInput { get; set; } = userInput;
        private readonly ResourceLoader _resourceLoader = resourceLoader ?? ResourceLoader.GetForViewIndependentUse();

        public string ToOptimizedRequest()
        {
            return _resourceLoader.GetString("/LLMResources/TargetTextFlagForOptimizingText") + Environment.NewLine
                + TargetText + Environment.NewLine
                + _resourceLoader.GetString("/LLMResources/UserInstructionFlagForOptimizingText") + Environment.NewLine
                + UserInput;
        }
        public override string ToString()
        {
            return ToOptimizedRequest();
        }
    }
}
