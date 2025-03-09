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


namespace AIPaste.Services.LLMServices
{
    internal class LocalLlmStrategy : ILlmStrategy
    {
        private LocalLlmSingleton _llmInstance;

        public LocalLlmStrategy(LLMLocalModelSettings modelSettings)
        {
            try
            {
                _llmInstance = LocalLlmSingleton.GetInstance(modelSettings);
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
            builder.Services.AddKeyedSingleton<IChatCompletionService>("local-llama", new LLamaSharpChatCompletion(ex));
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
    }
}
