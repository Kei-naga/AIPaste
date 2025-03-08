using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.Settings;
using Microsoft.SemanticKernel;

namespace AIPaste.Services.LLMServices
{
    internal interface ILlmStrategy
    {
        PromptExecutionSettings GetPromptExecutionSettings();
        IKernelBuilder GetKernelBuilder();
        static abstract bool CheckSettingsIntegrity(ILLMModelSettings modelSettings);
    }
}
