using System.Collections.Generic;
using System;
using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NLog;
using AIPaste.Models.DataModels;
using System.Threading.Tasks;

namespace AIPaste.Models.LLMModels
{
    internal class LlmTextCorrector : ILlmTextCorrector
    {
        public string PresentResponse { get; set; } = string.Empty;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ChatHistory _chatHistory;
        private readonly string _systemPrompt;
        public ChatHistory ChatHistory { get { return _chatHistory; } }
        private readonly PromptExecutionSettings _promptExecutionSettings;
        private readonly ILlmStrategy _llmStrategy;

        private readonly Logger _logger;

        public LlmTextCorrector(ILlmStrategy llmStrategy, string systemPrompt, ChatHistory? chatHistory = null , Logger? logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();
            _llmStrategy = llmStrategy;
            _systemPrompt = systemPrompt;
            _promptExecutionSettings = _llmStrategy.GetPromptExecutionSettings();
            var builder = _llmStrategy.GetKernelBuilder();
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _chatHistory = chatHistory ?? new ChatHistory();
            _chatHistory.AddSystemMessage(_systemPrompt);
        }

        public void ResetChat()
        {
            _chatHistory.Clear();
            _chatHistory.AddSystemMessage(_systemPrompt);
        }

        public async IAsyncEnumerable<string> GeneratingText(LlmRequest requestModel)
        {
            _chatHistory.AddUserMessage(requestModel.ToOptimizedRequest());
            TruncateChatHistoryByTokenLimit();
            var responseBuilder = new StringBuilder();
            var results = _chatCompletionService.GetStreamingChatMessageContentsAsync
                (
                    _chatHistory,
                    _promptExecutionSettings,
                    _kernel
                );
            await foreach (var result in results)
            {
                var text = result?.Content ?? string.Empty;
                responseBuilder.Append(text);
                yield return text;
            }
            _chatHistory.AddAssistantMessage(responseBuilder.ToString());
            PresentResponse = responseBuilder.ToString();
        }

        private void TruncateChatHistoryByTokenLimit()
        {
            int contextSize = _llmStrategy.GetTokenCount(_chatHistory);
            while (contextSize > _llmStrategy.ModelSettings.MaxContextSize)
            {
                _chatHistory.RemoveAt(0);
                contextSize = _llmStrategy.GetTokenCount(_chatHistory);
            }
        }


        public bool CheckIntegrity()
        {
            return Task.Run(() =>
                {
                    return CheckConnection();
                }).GetAwaiter().GetResult();
        }

        private async Task<bool> CheckConnection()
        {
            var testHistory = new ChatHistory();
            testHistory.AddSystemMessage("you are my assistant");
            testHistory.AddUserMessage("Hello!");
            try
            {
                var testTask = _chatCompletionService.GetChatMessageContentAsync(testHistory, kernel: _kernel);
                var result = await testTask;
                if (!(result.Items.Count > 0))
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
