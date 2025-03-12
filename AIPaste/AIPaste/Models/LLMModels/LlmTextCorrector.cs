using System.Collections.Generic;
using System;
using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using NLog;
using AIPaste.Models.DataModels;

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


        public static bool CheckSettingsIntegrity(ILLMModelSettings modelSettings)
        {
            if (modelSettings is GeminiModelSettings geminiModelSettings)
            {
                return GeminiStrategy.CheckSettingsIntegrity(geminiModelSettings);
            }
            else if (modelSettings is LLMLocalModelSettings localModelSettings)
            {
                return LocalLlmStrategy.CheckSettingsIntegrity(localModelSettings);
            }
            else
            {
                return false;
            }
        }
    }
}
