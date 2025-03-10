using System;
using AIPaste.Models.Settings;
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


namespace AIPaste.Services.LLMServices
{
    internal class LocalLlmStrategy : ILlmStrategy
    {
        private LocalLlmSingleton _llmInstance;
        private readonly IHistoryTransform _historyTransform;
        private readonly LLamaSharpPromptExecutionSettings _promptExecutionSettings;
        public ILLMModelSettings ModelSettings => _llmInstance.ModelSettings;

        public LocalLlmStrategy(LLMLocalModelSettings modelSettings)
        {
            try
            {
                _llmInstance = LocalLlmSingleton.GetInstance(modelSettings);
                _historyTransform = new HistoryTransform();
                _promptExecutionSettings = new LLamaSharpPromptExecutionSettings()
                {
                    MaxTokens = _llmInstance.ModelSettings.MaxTokens,
                    Temperature = 0.0,
                    TopP = 0.0,
                    StopSequences = new List<string>()
                };
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex, "Failed to initialize local model");
                throw;
            }
        }

        public IKernelBuilder GetKernelBuilder()
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
            return builder;
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

        public static bool CheckSettingsIntegrity(ILLMModelSettings modelSettings)
        {
            if (modelSettings is LLMLocalModelSettings localModelSettings)
            {
                try
                {
                    LocalLlmSingleton.GetInstance(localModelSettings);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public int GetTokenCount(Microsoft.SemanticKernel.ChatCompletion.ChatHistory chatHistory)
        {
            string text = _historyTransform.HistoryToText(chatHistory.ToLLamaSharpChatHistory());
            int tokenCount = _llmInstance.Localmodel.Tokenize(text,true,false, _llmInstance.Parameters.Encoding).Length;
            return tokenCount;
        }
    }
}
