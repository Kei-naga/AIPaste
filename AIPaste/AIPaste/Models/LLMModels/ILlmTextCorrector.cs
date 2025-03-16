using System.Collections.Generic;
using AIPaste.Models.DataModels;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    public interface ILlmTextCorrector
    {
        string PresentResponse { get; }
        ChatHistory ChatHistory { get; }
        void ResetChat();
        IAsyncEnumerable<string> GeneratingText(ILlmRequest requestModel);
        bool CheckIntegrity();
    }
}
