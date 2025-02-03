using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.LLMModels;
using AIPaste.Models.Settings;

namespace AIPaste.Services.LLMServices
{
    internal interface ILLMProvider
    {
        string PresentResponse { get; }
        void SetSystemPrompt(string SystemPrompt);
        // static abstract ILLMProvider Instance(LLMModelSettings modelSettings);
        void StartNewChat();
        void AddChatHistory(LlmRequestModel modelReq, string modelAns);
        IAsyncEnumerable<string> GeneratingText(LlmRequestModel req);
    }
}
