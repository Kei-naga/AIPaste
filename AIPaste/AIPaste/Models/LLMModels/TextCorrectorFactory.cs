using System;
using System.Linq;
using AIPaste.common;
using AIPaste.Models.DTO;
using LLamaSharp.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    public class TextCorrectorFactory : ITextCorrectorFactory
    {
        public ILlmTextCorrector CreateLlmTextCorrector(IAppSettings appSettings, IResourceLoaderWrapper? resourceLoader = null, IMyLogger? logger = null)
        {
            resourceLoader ??= new ResourceLoaderWrapper();
            var llmStrategy = GetLlmStrategy(appSettings);
            var systemPrompt = resourceLoader.GetString("/LLMResources/SystemPrompt");
            return new LlmTextCorrector(llmStrategy, systemPrompt, logger: logger);
        }

        private ILlmStrategy GetLlmStrategy(IAppSettings appSettings)
        {
            switch (appSettings.ActiveModelType)
            {
                case ModelType.LocalLLM:
                    var historyTransform = new HistoryTransform();
                    var localLlmSettings = appSettings.ModelSettingsList.FirstOrDefault(x => x is LlmLocalModelSettings) as LlmLocalModelSettings 
                        ?? throw new Exception("Local LLM settings not found");
                    return new LocalLlmStrategy(localLlmSettings, historyTransform);
                case ModelType.Gemini:
                    var geminiSettings = appSettings.ModelSettingsList.FirstOrDefault(x => x is GeminiModelSettings) as GeminiModelSettings
                        ?? throw new Exception("Gemini settings not found");
                    return new GeminiStrategy(geminiSettings);
                default:
                    throw new Exception("not find model type");
            }
        }
    }

    public interface ITextCorrectorFactory
    {
        ILlmTextCorrector CreateLlmTextCorrector(IAppSettings appSettings, IResourceLoaderWrapper? resourceLoader = null, IMyLogger? logger = null);
    }
}
