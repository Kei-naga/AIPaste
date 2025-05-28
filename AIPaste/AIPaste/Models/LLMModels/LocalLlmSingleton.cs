using LLama.Common;
using LLama;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPaste.Models.LLMModels
{
    internal class LocalLlmSingleton
    {
        private static LocalLlmSingleton? _instance;
        private static readonly object _lock = new();
        private LocalLlmSingleton(LlmLocalModelSettings modelSettings)
        {
            ModelSettings = modelSettings;
            Parameters = new ModelParams(ModelSettings.ModelPath)
            {
                ContextSize = ModelSettings.MaxContextSize,
                GpuLayerCount = ModelSettings.GpuLayerCount
            };
        }

        public static LocalLlmSingleton GetInstance(LlmLocalModelSettings modelSettings)
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

        public LlmLocalModelSettings ModelSettings { get; private set; }
        public ModelParams Parameters { get; private set; }

        public LLamaWeights Localmodel { get 
            {
                _localmodel ??= LLamaWeights.LoadFromFile(Parameters);
                return _localmodel;
            }}

        private LLamaWeights? _localmodel;

        public LLamaWeights ReloadModel()
        {
            _localmodel?.Dispose();
            _localmodel = LLamaWeights.LoadFromFile(Parameters);
            return _localmodel;
        }

        public static void Dispose()
        {
            _instance?._localmodel?.Dispose();
            _instance = null;
        }
    }
}
