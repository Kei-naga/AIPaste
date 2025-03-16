using System;
using AIPaste.common;
using AIPaste.Models.DataModels;
using LLamaSharp.SemanticKernel.ChatCompletion;
using NLog;

namespace AIPaste.Models.LLMModels
{
    public class TextCorrectorFactory : ITextCorrectorFactory
    {
        public ILlmTextCorrector CreateLlmTextCorrector(AppSettings appSettings, IResourceLoaderWrapper? resourceLoader = null, ILogger? logger = null)
        {
            resourceLoader ??= new ResourceLoaderWrapper();
            var llmStrategy = GetLlmStrategy(appSettings);
            var systemPrompt = resourceLoader.GetString("/LLMResources/SystemPrompt");
            return new LlmTextCorrector(llmStrategy, systemPrompt, logger: logger);
        }

        private ILlmStrategy GetLlmStrategy(AppSettings appSettings)
        {
            switch (appSettings.ModelType)
            {
                case ModelType.LocalLLM:
                    var historyTransform = new HistoryTransform();
                    return new LocalLlmStrategy(appSettings.LocalLLMSettings, historyTransform);
                case ModelType.Gemini:
                    return new GeminiStrategy(appSettings.GeminiSettings);
                default:
                    throw new Exception("not find model type");
            }
        }
    }

    public interface ITextCorrectorFactory
    {
        ILlmTextCorrector CreateLlmTextCorrector(AppSettings appSettings, IResourceLoaderWrapper? resourceLoader = null, ILogger? logger = null);
    }
}
