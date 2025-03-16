using AIPaste.Models.DataModels;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;

// using Microsoft.SemanticKernel.Connectors.Google;
#pragma warning disable SKEXP0070

namespace AIPaste.Models.LLMModels
{
    public class GeminiStrategy(GeminiModelSettings modelSettings) : ILlmStrategy
    {
        private readonly GeminiModelSettings _modelSettings = modelSettings;
        public ILLMModelSettings ModelSettings => _modelSettings;
        public ModelType ModelType => ModelType.Gemini;

        public Kernel GetKernel()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddGoogleAIGeminiChatCompletion(
                modelId: _modelSettings.ModelName,
                apiKey: _modelSettings.ApiKey);
            return builder.Build();
        }
        public PromptExecutionSettings GetPromptExecutionSettings()
        {
            return new GeminiPromptExecutionSettings();
        }

        public int GetTokenCount(ChatHistory chatHistory)
        {
            // TODO: Implement this method
            return 0;
        }
    }
}
