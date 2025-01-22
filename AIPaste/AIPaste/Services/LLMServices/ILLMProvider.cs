using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPaste.Services.LLMServices
{
    internal interface ILLMProvider
    {
        string PresentResponse { get; }
        void Initialize();
        void SetSystemPrompt(string SystemPrompt);
        void StartChat();
        void AddChatHistory(string modelReq, string modelAns);
        IAsyncEnumerable<string> GeneratingText(string req);
    }
}
