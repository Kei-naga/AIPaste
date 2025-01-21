using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPaste.Services.LLMServices
{
    internal interface LLMService
    {
        string PresentResponse { get; }
        void Initialize();
        void StartChat(string SystemPrompt, string modelReq, string modelAns);
        IAsyncEnumerable<string> GeneratingText(string req);
    }
}
