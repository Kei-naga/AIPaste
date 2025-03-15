using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SemanticKernel.ChatCompletion;
using AIPaste.Models.DataModels;


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
