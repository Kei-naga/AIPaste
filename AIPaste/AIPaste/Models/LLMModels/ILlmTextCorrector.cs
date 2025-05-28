using System.Collections.Generic;
using Microsoft.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    public interface ILlmTextCorrector
    {
        string PresentResponse { get; }
        ChatHistory ChatHistory { get; }
        void ResetChat();
        IAsyncEnumerable<string> GeneratingText(string targetText, string userInput);
        bool CheckIntegrity();
    }
}
