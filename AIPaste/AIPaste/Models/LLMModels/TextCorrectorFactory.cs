using System;
using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;
using LLamaSharp.SemanticKernel.ChatCompletion;

namespace AIPaste.Models.LLMModels
{
    public class TextCorrectorFactory : ITextCorrectorFactory
    {
        public ILlmTextCorrector CreateLlmTextCorrector(ILlmModelSettings llmModelSettings, IResourceLoaderWrapper? resourceLoader = null, IMyLogger? logger = null)
        {
            resourceLoader ??= new ResourceLoaderWrapper();
            var llmStrategy = GetLlmStrategy(llmModelSettings);
            var systemPrompt = resourceLoader.GetString("/LLMResources/SystemPrompt");
            return new LlmTextCorrector(llmStrategy, systemPrompt, logger: logger);
        }

        private ILlmStrategy GetLlmStrategy(ILlmModelSettings llmModelSettings)
        {
            var historyTransform = new HistoryTransform();
            if (llmModelSettings is LlmLocalModelSettings localLlmSettings)
            { 
                return new LocalLlmStrategy(localLlmSettings, historyTransform);
            }
            else if (llmModelSettings is GeminiModelSettings geminiSettings)
            { 
                return new GeminiStrategy(geminiSettings);
            }
            else
            { 
                throw new Exception("not find model type");
            }
        }
    }

    public interface ITextCorrectorFactory
    {
        ILlmTextCorrector CreateLlmTextCorrector(ILlmModelSettings llmModelSettings, IResourceLoaderWrapper? resourceLoader = null, IMyLogger? logger = null);
    }
}
