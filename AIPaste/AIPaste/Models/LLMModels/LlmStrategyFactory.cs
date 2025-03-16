using System;
using AIPaste.Models.DataModels;
using LLamaSharp.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    internal class LlmStrategyFactory(AppSettings appSettings)
    {
        private readonly AppSettings _appSettings = appSettings;

        public ILlmStrategy GetLlmStrategy()
        {
            switch (_appSettings.ModelType)
            {
                case ModelType.LocalLLM:
                    var historyTransform = new HistoryTransform();
                    return new LocalLlmStrategy(_appSettings.LocalLLMSettings, historyTransform);
                case ModelType.Gemini:
                    return new GeminiStrategy(_appSettings.GeminiSettings);
                default:
                    throw new Exception("not find model type");
            }
        }
    }
}
