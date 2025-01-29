using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.Settings;

namespace AIPaste.Services.LLMServices
{
    class GeminiProvider(GeminiModelSettings modelSettings): ILLMProvider
    {
        public string PresentResponse { get; } = "";
        private GeminiModelSettings _modelSettings = modelSettings;

        public void Initialize()
        {
            throw new NotImplementedException();
        }
        public void SetSystemPrompt(string SystemPrompt)
        {
            throw new NotImplementedException();
        }
        public void StartNewChat()
        {
            throw new NotImplementedException();
        }
        public void AddChatHistory(string modelReq, string modelAns)
        {
            throw new NotImplementedException();
        }
        public IAsyncEnumerable<string> GeneratingText(string req)
        {
            throw new NotImplementedException();
        }
    }
}
