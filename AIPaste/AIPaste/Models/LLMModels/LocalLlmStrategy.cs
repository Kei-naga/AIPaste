using LLama;
using LLamaSharp.SemanticKernel.ChatCompletion;
using LLamaSharp.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using LLama.Abstractions;
using AIPaste.Models.DTO;
using System.Collections.Generic;

namespace AIPaste.Models.LLMModels
{
    public class LocalLlmStrategy : ILlmStrategy
    {
        private LocalLlmSingleton _llmInstance;
        private readonly IHistoryTransform _historyTransform;
        private readonly LLamaSharpPromptExecutionSettings _promptExecutionSettings;
        public ILlmModelSettings ModelSettings => _llmInstance.ModelSettings;
        public ModelType ModelType => ModelType.LocalLLM;

        public LocalLlmStrategy(LlmLocalModelSettings modelSettings, IHistoryTransform historyTransform)
        {
            _llmInstance = LocalLlmSingleton.GetInstance(modelSettings);
            _historyTransform = historyTransform;
            _promptExecutionSettings = new LLamaSharpPromptExecutionSettings()
            {
                MaxTokens = modelSettings.MaxTokens,
                Temperature = 0.0,
                TopP = 0.0,
                StopSequences = new List<string>()
            };
        }

        public Kernel GetKernel()
        {
            var builder = Kernel.CreateBuilder();
            var ex = new StatelessExecutor(_llmInstance.Localmodel, _llmInstance.Parameters);
            builder.Services.AddKeyedSingleton<IChatCompletionService>(
                "local-llama",
                new LLamaSharpChatCompletion(
                    model: ex,
                    defaultRequestSettings: _promptExecutionSettings,
                    historyTransform: _historyTransform
                )
            );
            return builder.Build();
        }

        public PromptExecutionSettings GetPromptExecutionSettings()
        {
            return new LLamaSharpPromptExecutionSettings()
            {
                MaxTokens = _llmInstance.ModelSettings.MaxTokens,
            };
        }

        public static void Dispose()
        {
            LocalLlmSingleton.Dispose();
        }

        public int GetTokenCount(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
        {
            string text = _historyTransform.HistoryToText(chatHistory.ToLLamaSharpChatHistory());
            int tokenCount = _llmInstance.Localmodel.Tokenize(text, true, false, _llmInstance.Parameters.Encoding).Length;
            return tokenCount;
        }
    }
}
