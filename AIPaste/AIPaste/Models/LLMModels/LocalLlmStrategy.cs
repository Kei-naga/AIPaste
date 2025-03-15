using System;
using LLama;
using LLamaSharp.SemanticKernel.ChatCompletion;
using LLamaSharp.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using LLama.Common;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using ManagedCuda;
using LLama.Abstractions;
using System.Collections.Generic;
using System.Text;
using AIPaste.Models.DataModels;


namespace AIPaste.Models.LLMModels
{
    public class LocalLlmStrategy : ILlmStrategy
    {
        private LocalLlmSingleton _llmInstance;
        private readonly IHistoryTransform _historyTransform;
        private readonly LLamaSharpPromptExecutionSettings _promptExecutionSettings;
        public ILLMModelSettings ModelSettings => _llmInstance.ModelSettings;
        public ModelType ModelType => ModelType.LocalLLM;

        public LocalLlmStrategy(LocalLlmSingleton localLlmSingleton, IHistoryTransform historyTransform, LLamaSharpPromptExecutionSettings llamaSharpPromptExecutionSettings)
        {
            _llmInstance = localLlmSingleton;
            _historyTransform = historyTransform;
            _promptExecutionSettings = llamaSharpPromptExecutionSettings;
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
