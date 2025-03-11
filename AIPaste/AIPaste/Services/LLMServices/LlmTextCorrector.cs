using AIPaste.Models.Settings;
using System.Collections.Generic;
using System;
using Windows.ApplicationModel.Resources;
using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIPaste.Models.LLMModels;
using NLog;

namespace AIPaste.Services.LLMServices
{
    internal class LlmTextCorrector
    {
        public string PresentResponse = "";
        private ModelType _modelType;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private Microsoft.SemanticKernel.ChatCompletion.ChatHistory _chatHistory;
        private readonly PromptExecutionSettings _promptExecutionSettings;
        private readonly ILlmStrategy _llmStrategy;

        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public LlmTextCorrector(AppSettings appSettings)
        {
            _modelType = appSettings.ModelType;

            if (_modelType == ModelType.LocalLLM)
            {
                _llmStrategy = new LocalLlmStrategy(appSettings.LocalLLMSettings);
            }
            else
            {
                _llmStrategy = new GeminiStrategy(appSettings.GeminiSettings);
            }
            _promptExecutionSettings = _llmStrategy.GetPromptExecutionSettings();
            var builder = _llmStrategy.GetKernelBuilder();
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _chatHistory = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory(_resourceLoader.GetString("/LLMResources/SystemPrompt"));
        }

        public void ResetChat() {
            _chatHistory.Clear();
            _chatHistory.AddSystemMessage(_resourceLoader.GetString("/LLMResources/SystemPrompt"));
        }

        public async IAsyncEnumerable<string> GeneratingText(LlmRequestModel requestModel)
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


        public static bool CheckSettingsIntegrity(ILLMModelSettings modelSettings) { 
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
