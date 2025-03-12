using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.DataModels;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    internal interface ILlmStrategy
    {
        ModelType ModelType { get; }
        PromptExecutionSettings GetPromptExecutionSettings();
        IKernelBuilder GetKernelBuilder();
        static abstract bool CheckSettingsIntegrity(ILLMModelSettings modelSettings);
        int GetTokenCount(ChatHistory chatHistory);
        ILLMModelSettings ModelSettings { get; }
    }
}
