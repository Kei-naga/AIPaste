using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AIPaste.Models.Settings;
using AIPaste.Services.LLMServices;
using AIPaste.Services.SettingsServices;
using NLog;

namespace AIPaste.ViewModels
{
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        private AppSettings _appSettings;
        private SettingsService _settingsService;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public SettingsPageViewModel()
        {
            _settingsService = new SettingsService();
            _appSettings = _settingsService.LoadSettings();

            LLMModelPath = _appSettings.LocalLLMSettings.ModelPath;
            GpuLayerCount = _appSettings.LocalLLMSettings.GpuLayerCount;
            MaxTokens = _appSettings.LocalLLMSettings.MaxTokens;
            GpuEnabled = _appSettings.LocalLLMSettings.GpuEnabled;
            KeyPattern = _appSettings.KeySettings.KeyPattern;
            AutoStart = _appSettings.AutoStart;
            ModelType = _appSettings.ModelType;
            ApiKey = _appSettings.GeminiSettings.ApiKey;
        }

        public List<Tuple<string, ModelType>> ModelTypes = Enum.GetValues(typeof(ModelType))
                .Cast<ModelType>()
                .Select(mt => new Tuple<string, ModelType>(mt.ToString(), mt))
                .ToList();

        private bool _settingsChanged = false;

        private string _llmModelPath = "";
        public string LLMModelPath
        {
            get => _llmModelPath;
            set
            {
                if (_llmModelPath != value)
                {
                    _llmModelPath = value;
                    OnPropertyChanged(nameof(LLMModelPath));
                    _settingsChanged = true;
                }
            }
        }

        private int _gpuLayerCount = 32;
        public int GpuLayerCount
        {
            get => _gpuLayerCount;
            set
            {
                if (_gpuLayerCount != value)
                {
                    _gpuLayerCount = value;
                    OnPropertyChanged(nameof(GpuLayerCount));
                    _settingsChanged = true;
                }
            }
        }

        private int _maxTokens = 1024;
        public int MaxTokens
        {
            get => _maxTokens;
            set
            {
                if (_maxTokens != value)
                {
                    _maxTokens = value;
                    OnPropertyChanged(nameof(MaxTokens));
                    _settingsChanged = true;
                }
            }
        }

        private bool _gpuEnabled = false;
        public bool GpuEnabled
        {
            get => _gpuEnabled;
            set
            {
                if (_gpuEnabled != value)
                {
                    _gpuEnabled = value;
                    OnPropertyChanged(nameof(GpuEnabled));
                    _settingsChanged = true;
                }
            }
        }

        private string _keyPattern = "Ctrl+Shift+V";
        public string KeyPattern
        {
            get => _keyPattern;
            set
            {
                if (_keyPattern != value)
                {
                    _keyPattern = value;
                    OnPropertyChanged(nameof(KeyPattern));
                    _settingsChanged = true;
                }
            }
        }

        private bool _autoStart = true;
        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                if (_autoStart != value)
                {
                    _autoStart = value;
                    OnPropertyChanged(nameof(AutoStart));
                    _settingsChanged = true;
                }
            }
        }

        private ModelType _modelType = ModelType.LocalLLM;
        public ModelType ModelType
        {
            get => _modelType;
            set
            {
                if (_modelType != value)
                {
                    _modelType = value;
                    OnPropertyChanged(nameof(ModelType));
                    _settingsChanged = true;
                }
            }
        }

        private string _apiKey = "";
        public string ApiKey
        {
            get => _apiKey;
            set
            {
                if (_apiKey != value)
                {
                    _apiKey = value;
                    OnPropertyChanged(nameof(ApiKey));
                    _settingsChanged = true;
                }
            }
        }

        public void SaveSettings()
        {
            if (!_settingsChanged)
            {
                _logger.Debug("No settings changed, not saving");
                return;
            }
            if (!IsValidSettings())
            {
                _logger.Debug("Invalid Settings");
                return;
            }
            _settingsChanged = false;
            _logger.Info("Saving new Settings");
            var localModelSettings = new LLMLocalModelSettings(
                ModelPath: LLMModelPath,
                GpuEnable: GpuEnabled,
                GpuLayerCount: GpuLayerCount,
                ContextSize: _appSettings.LocalLLMSettings.ContextSize,
                AntiPrompts: _appSettings.LocalLLMSettings.AntiPrompts,
                MaxTokens: MaxTokens
            );
            var geminiModelSettings = new GeminiModelSettings(ApiKey);
            var keySettings = new KeySettings(KeyPattern);
            var newSettings = new AppSettings(
                AutoStart,
                ModelType,
                keySettings,
                localModelSettings,
                geminiModelSettings
            );
            _settingsService.SaveSettings(newSettings);
            if (ModelType != ModelType.LocalLLM)
            {
                _logger.Info("Despose Local LLM");
                LocalLLMProvider.Dispose();
            }
        }

        private bool IsValidSettings()
        {
            ILLMModelSettings modelSettings;
            if (ModelType == ModelType.LocalLLM)
            {
                modelSettings = new LLMLocalModelSettings(
                    ModelPath: LLMModelPath,
                    GpuEnable: GpuEnabled,
                    GpuLayerCount: GpuLayerCount,
                    ContextSize: _appSettings.LocalLLMSettings.ContextSize,
                    AntiPrompts: _appSettings.LocalLLMSettings.AntiPrompts,
                    MaxTokens: MaxTokens
                );
                return LocalLLMProvider.CheckSettingsIntegrity(modelSettings);
            }
            else if (ModelType == ModelType.Gemini)
            {
                modelSettings = new GeminiModelSettings(ApiKey);
                // TODO: Implement GeminiProvider to check settings integrity
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}