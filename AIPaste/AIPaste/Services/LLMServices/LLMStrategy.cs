using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Common;
using LLama;
using AIPaste.Models.Settings;
using AIPaste.Models.LLMModels;
using Windows.ApplicationModel.Resources;

namespace AIPaste.Services.LLMServices
{
    internal class LLMStrategy
    {
        private readonly ResourceLoader _resourceLoader = new ResourceLoader();

        public (LlmRequestModel, string) CreateModelPrompt()
        {
            var modelTargetText = _resourceLoader.GetString("/LLMResources/SampleTargetText1");
            var modelInput = _resourceLoader.GetString("/LLMResources/SampleUserInstruction1");
            var modelPrompt = new LlmRequestModel(modelTargetText, modelInput);
            string modelAns = _resourceLoader.GetString("/LLMResources/SampleAns1");
            return (modelPrompt, modelAns);
        }

        public string CreateOptimizedReq(string targetText, string input)
        {
            return _resourceLoader.GetString("/LLMResources/TargetTextFlagForOptimizingText") + Environment.NewLine
                + targetText + Environment.NewLine
                + _resourceLoader.GetString("/LLMResources/UserInstructionFlagForOptimizingText") + Environment.NewLine 
                + input;
        }
        public string GetSystemPrompt()
        {
            return _resourceLoader.GetString("/LLMResources/SystemPrompt");
        }
    }

}
