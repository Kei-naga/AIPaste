using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Common;
using LLama;
using AIPaste.Models.DataModels;

namespace AIPaste.Models.LLMModels
{
    internal class LocalLlmSingleton
    {
        private static LocalLlmSingleton? _instance;
        private static readonly object _lock = new();
        private LocalLlmSingleton(LLMLocalModelSettings modelSettings)
        {
            ModelSettings = modelSettings;
            Parameters = new ModelParams(ModelSettings.ModelPath)
            {
                ContextSize = ModelSettings.MaxContextSize,
                GpuLayerCount = ModelSettings.GpuLayerCount
            };
            Localmodel = LLamaWeights.LoadFromFile(Parameters);
        }

        public static LocalLlmSingleton GetInstance(LLMLocalModelSettings modelSettings)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance = new LocalLlmSingleton(modelSettings);
                }
            }
            else if (!modelSettings.Equals(_instance.ModelSettings))
            {
                lock (_lock)
                {
                    Dispose();
                    _instance = new LocalLlmSingleton(modelSettings);
                }
            }
            return _instance;
        }

        public LLMLocalModelSettings ModelSettings { get; private set; }
        public LLamaWeights Localmodel { get; private set; }
        public ModelParams Parameters { get; private set; }

        public static void Dispose()
        {
            _instance?.Localmodel.Dispose();
            _instance = null;
        }
    }
}
