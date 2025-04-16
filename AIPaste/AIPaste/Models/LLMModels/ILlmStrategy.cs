using AIPaste.Models.DTO;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    public interface ILlmStrategy
    {
        ModelType ModelType { get; }
        PromptExecutionSettings GetPromptExecutionSettings();
        Kernel GetKernel();
        int GetTokenCount(ChatHistory chatHistory);
        ILlmModelSettings ModelSettings { get; }
    }
}
