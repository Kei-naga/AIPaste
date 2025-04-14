using System.Collections.Generic;
using System;
using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIPaste.Models.DataModels;
using System.Threading.Tasks;
using System.Linq;
using AIPaste.common;

namespace AIPaste.Models.LLMModels
{
    public class LlmTextCorrector : ILlmTextCorrector
    {
        public string PresentResponse { get; private set; } = string.Empty;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ChatHistory _chatHistory;
        private readonly string _systemPrompt;
        public ChatHistory ChatHistory => _chatHistory;
        private readonly PromptExecutionSettings _promptExecutionSettings;
        private readonly ILlmStrategy _llmStrategy;
        private readonly IResourceLoaderWrapper _resourceLoader;

        private readonly IMyLogger _logger;

        public LlmTextCorrector(ILlmStrategy llmStrategy, string systemPrompt , IResourceLoaderWrapper? resourceLoader = null, IMyLogger? logger = null)
        {
            _logger = logger ?? MyLogger.GetInstance();
            _resourceLoader = resourceLoader ?? new ResourceLoaderWrapper();
            _llmStrategy = llmStrategy;
            _systemPrompt = systemPrompt;
            _promptExecutionSettings = _llmStrategy.GetPromptExecutionSettings();
            _kernel = _llmStrategy.GetKernel();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _chatHistory = [];
            ResetChat();
        }

        public void ResetChat()
        {
            _chatHistory.Clear();
            _chatHistory.AddSystemMessage(_systemPrompt);
            DoOneShotLearning();
        }

        private void DoOneShotLearning()
        {
            var oneShotTarget = _resourceLoader.GetString("/LLMResources/OneShot_TargetText");
            var oneShotInstruction = _resourceLoader.GetString("/LLMResources/OneShot_Instruction");
            var oneShotAns = _resourceLoader.GetString("/LLMResources/OneShot_Ans");
            var oneShotRequest = new LlmRequest(oneShotTarget, oneShotInstruction, _resourceLoader);
            _chatHistory.AddUserMessage(oneShotRequest.ToOptimizedRequest());
            _chatHistory.AddAssistantMessage(oneShotAns);
        }

        /// <summary>
        /// Generate text using the LLM model with streaming support.
        /// </summary>
        public async IAsyncEnumerable<string> GeneratingText(ILlmRequest requestModel)
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

        /// <summary>
        /// Check the integrity of the LLM model by testing the connection.
        /// </summary>
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
                if (String.IsNullOrEmpty(result.Items.OfType<TextContent>().FirstOrDefault()?.Text ?? null))
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
