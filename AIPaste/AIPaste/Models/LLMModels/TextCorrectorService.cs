using System.Collections.Generic;
using AIPaste.Models.DataModels;
using Microsoft.SemanticKernel.ChatCompletion;
using NLog;

namespace AIPaste.Models.LLMModels
{
    public class TextCorrectService : ITextCorrectService
    {
        private readonly LlmTextCorrector _textCorrector;
        public TextCorrectService(AppSettings appSettings, string systemPrompt, ILogger? logger = null)
        {
            var llmStrategy = new LlmStrategyFactory(appSettings).GetLlmStrategy();
            _textCorrector = new LlmTextCorrector(llmStrategy, systemPrompt, logger: logger);
        }
        public string PresentResponse { get => _textCorrector.PresentResponse; }
        public ChatHistory ChatHistory { get => _textCorrector.ChatHistory; }
        public void ResetChat() => _textCorrector.ResetChat();
        public async IAsyncEnumerable<string> GeneratingText(ILlmRequest requestModel)
        {
            await foreach (var text in _textCorrector.GeneratingText(requestModel))
            {
                yield return text;
            }
        }
        public bool CheckIntegrity() => _textCorrector.CheckIntegrity();
    }
}
