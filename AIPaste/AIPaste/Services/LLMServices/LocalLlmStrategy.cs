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


namespace AIPaste.Services.LLMServices
{
    internal class LocalLlmStrategy : ILlmStrategy
    {
        private static LLMLocalModelSettings? _modelSettings;
        private static LLamaWeights? _localmodel;
        private readonly ModelParams _parameters;

        public LocalLlmStrategy(LLMLocalModelSettings modelSettings)
        {
            if (_modelSettings == null || !modelSettings.Equals(_modelSettings))
            {
                _modelSettings = modelSettings;
                _parameters = new ModelParams(_modelSettings.ModelPath)
                {
                    ContextSize = _modelSettings.ContextSize,
                    GpuLayerCount = _modelSettings.GpuLayerCount
                };
                _localmodel?.Dispose();
                _localmodel = LLamaWeights.LoadFromFile(_parameters);
            }
            else
            {
                _parameters = new ModelParams(_modelSettings.ModelPath)
                {
                    ContextSize = _modelSettings.ContextSize,
                    GpuLayerCount = _modelSettings.GpuLayerCount
                };
            }
        }
        public IKernelBuilder GetKernelBuilder()
        {
            var builder = Kernel.CreateBuilder();
            if (_localmodel == null)
            {
                throw new InvalidOperationException("Local model is not loaded");
            }
            var ex = new StatelessExecutor(_localmodel, _parameters);
            builder.Services.AddKeyedSingleton<IChatCompletionService>("local-llama", new LLamaSharpChatCompletion(ex));
            return builder;
        }
        public PromptExecutionSettings GetPromptExecutionSettings()
        {
            return new LLamaSharpPromptExecutionSettings()
            {
                MaxTokens = _modelSettings?.MaxTokens,
            };
        }
        public static void Dispose()
        {
            _localmodel?.Dispose();
            _localmodel = null;
            _modelSettings = null;
        }
    }
}
