using System;
using System.Collections.Generic;
using AIPaste.Models.DataModels;
using LLamaSharp.SemanticKernel;
using LLamaSharp.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    internal class LlmStrategyFactory : ILlmStrategyFactory
    {
        private readonly AppSettings _appSettings;
        public LlmStrategyFactory(AppSettings appSettings) {
            _appSettings = appSettings;
        }

        public ILlmStrategy GetLlmStrategy()
        {
            if (_appSettings.ModelType == ModelType.LocalLLM)
            {
                var llmInstance = LocalLlmSingleton.GetInstance(_appSettings.LocalLLMSettings);
                var historyTransform = new HistoryTransform();
                var llamaSharpPromptExecutionSettings = new LLamaSharpPromptExecutionSettings()
                {
                    MaxTokens = _appSettings.LocalLLMSettings.MaxTokens,
                    Temperature = 0.0,
                    TopP = 0.0,
                    StopSequences = new List<string>()
                };
                return new LocalLlmStrategy(llmInstance, historyTransform, llamaSharpPromptExecutionSettings);
            }
            else if (_appSettings.ModelType == ModelType.Gemini)
            {
                return new GeminiStrategy(_appSettings.GeminiSettings);
            }
            else
            {
                throw new Exception("not find model type"); 
            }
        }
    }

    public interface ILlmStrategyFactory
    {
        ILlmStrategy GetLlmStrategy();
    }
}
