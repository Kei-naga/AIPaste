using AIPaste.Models.Settings;
using GenerativeAI.Types;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;


// using Microsoft.SemanticKernel.Connectors.OpenAI;
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0070

namespace AIPaste.Services.LLMServices
{
    internal class GeminiStrategy : ILlmStrategy
    {
        private readonly GeminiModelSettings _modelSettings;
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
    }
}
