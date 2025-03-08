using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;

namespace AIPaste.Services.LLMServices
{
    internal interface ILlmStrategy
    {
        PromptExecutionSettings GetPromptExecutionSettings();
        IKernelBuilder GetKernelBuilder();
    }
}
