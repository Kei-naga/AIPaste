using System;
using System.Threading.Tasks;
using AIPaste.Models.Settings;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Services;
using Microsoft.UI.Composition.Interactions;


// using Microsoft.SemanticKernel.Connectors.Google;
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0070

namespace AIPaste.Services.LLMServices
{
    internal class GeminiStrategy : ILlmStrategy
    {
        private readonly GeminiModelSettings _modelSettings;
        public ILLMModelSettings ModelSettings => _modelSettings;
        public GeminiStrategy(GeminiModelSettings modelSettings)
        {
            _modelSettings = modelSettings;
        }
        public IKernelBuilder GetKernelBuilder()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddGoogleAIGeminiChatCompletion(
                modelId: _modelSettings.ModelName,
                apiKey: _modelSettings.ApiKey);
            return builder;
        }
        public PromptExecutionSettings GetPromptExecutionSettings() {
            return new GeminiPromptExecutionSettings();
        }

        public static bool CheckSettingsIntegrity(ILLMModelSettings modelSettings)
        {
            if (modelSettings is GeminiModelSettings settings)
            {
                if (string.IsNullOrEmpty(settings.ApiKey) || string.IsNullOrEmpty(settings.ModelName))
                {
                    return false;
                }
                return Task.Run(() => 
                    {
                        return CheckConnection(settings.ApiKey, settings.ModelName);
                    }).GetAwaiter().GetResult();
            }
            return false;
        }

        private static async Task<bool> CheckConnection(string apiKey, string modeName)
        {
            var testHistory = new ChatHistory();
            testHistory.AddSystemMessage("you are my assistant");
            testHistory.AddUserMessage("Hello!");
            var testKernel = Kernel.CreateBuilder().AddGoogleAIGeminiChatCompletion(modeName, apiKey).Build();
            var geminiChatCompletion = testKernel.GetRequiredService<IChatCompletionService>();
            try
            {
                Task testTask = geminiChatCompletion.GetChatMessageContentAsync(testHistory, kernel: testKernel);
                await testTask;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public int GetTokenCount(ChatHistory chatHistory)
        {
            // TODO: Implement this method
            return 0;
        }
    }
}
