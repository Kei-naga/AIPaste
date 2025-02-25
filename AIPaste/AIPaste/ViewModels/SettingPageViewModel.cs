using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AIPaste.Models.Settings;
using AIPaste.Models.KeyModels;
using AIPaste.Services.LLMServices;
using AIPaste.Services.SettingsServices;
using NLog;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AIPaste.ViewModels
{
    public partial class SettingsPageViewModel : INotifyPropertyChanged
    {
        private AppSettings _appSettings;
        private SettingsService _settingsService;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SettingsPageViewModel()
        {
            _settingsService = new SettingsService();
            _appSettings = _settingsService.LoadSettings();
            
            UpdateSettings();
        }

        private void UpdateSettings(AppSettings? newSettings = null)
        {
            if (newSettings != null)
            {
                _appSettings = newSettings;
            }
            LLMModelPath = _appSettings.LocalLLMSettings.ModelPath;
            GpuLayerCount = _appSettings.LocalLLMSettings.GpuLayerCount;
            MaxTokens = _appSettings.LocalLLMSettings.MaxTokens;
            GpuEnabled = _appSettings.LocalLLMSettings.GpuEnabled;
            IsHotkeyEnabled = _appSettings.KeySettings.IsHotkeyEnabled;
            Key = _appSettings.KeySettings.KeyPattern.Key;
            CtrlModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            AltModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);
            ShiftModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            WinModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN);
            AutoStart = _appSettings.AutoStart;
            ModelTypeName = _appSettings.ModelType;
            ApiKey = _appSettings.GeminiSettings.ApiKey;
        }

        public List<Tuple<string, ModelType>> ModelTypes = Enum.GetValues(typeof(ModelType))
                .Cast<ModelType>()
                .Select(mt => new Tuple<string, ModelType>(mt.ToString(), mt))
                .ToList();

        public List<Tuple<string, VirtualKey>> VirtualKeys = Enum.GetValues(typeof(VirtualKey))
                .Cast<VirtualKey>()
                .Select(vk => new Tuple<string, VirtualKey>(vk.ToString(), vk))
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

        private bool _isHotkeyEnabled = true;
        public bool IsHotkeyEnabled
        {
            get => _isHotkeyEnabled;
            set
            {
                if (_isHotkeyEnabled != value)
                {
                    _isHotkeyEnabled = value;
                    OnPropertyChanged(nameof(IsHotkeyEnabled));
                    _settingsChanged = true;
                }
            }
        }

        private VirtualKey _key = VirtualKey.C;
        public VirtualKey Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    OnPropertyChanged(nameof(Key));
                    _settingsChanged = true;
                }
            }
        }

        private bool _ctrlModifier = true;
        public bool CtrlModifier
        {
            get => _ctrlModifier;
            set
            {
                if (_ctrlModifier != value)
                {
                    _ctrlModifier = value;
                        OnPropertyChanged(nameof(CtrlModifier));
                        _settingsChanged = true;
                }
            }
        }
        
        private bool _altModifier = true;
        public bool AltModifier
        {
            get => _altModifier;
            set
            {
                if (_altModifier != value)
                {
                    _altModifier = value;
                        OnPropertyChanged(nameof(AltModifier));
                        _settingsChanged = true;
                }
            }
        }

        private bool _shiftModifier = false;
        public bool ShiftModifier
        {
            get => _shiftModifier;
            set
            {
                if (_shiftModifier != value)
                {
                    _shiftModifier = value;
                        OnPropertyChanged(nameof(ShiftModifier));
                        _settingsChanged = true;
                }
            }
        }

        private bool _winModifier = false;
        public bool WinModifier
        {
            get => _winModifier;
            set
            {
                if (_winModifier != value)
                {
                    _winModifier = value;
                        OnPropertyChanged(nameof(WinModifier));
                        _settingsChanged = true;
                }
            }
        }

        private bool _autoStart = false;
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

        private ModelType _modelTypeName = ModelType.LocalLLM;
        public ModelType ModelTypeName
        {
            get => _modelTypeName;
            set
            {
                if (_modelTypeName != value)
                {
                    _modelTypeName = value;
                    OnPropertyChanged(nameof(ModelType));
                    _settingsChanged = true;
                    OnPropertyChanged(nameof(IsLocalLLMSelected));
                    OnPropertyChanged(nameof(IsGeminiSelected));
                }
            }
        }

        public bool IsLocalLLMSelected => ModelTypeName == ModelType.LocalLLM;
        public bool IsGeminiSelected => ModelTypeName == ModelType.Gemini;

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

        public bool SaveSettings()
        {
            if (!_settingsChanged)
            {
                _logger.Debug("No settings changed, not saving");
                return true;
            }
            if (!IsValidSettings())
            {
                _logger.Debug("Invalid Settings");
                UpdateSettings();
                return false;
            }
            _settingsChanged = false;
            _logger.Info("Saving new Settings");
            var localModelSettings = new LLMLocalModelSettings(
                ModelPath: LLMModelPath,
                GpuEnable: GpuEnabled,
                GpuLayerCount: GpuLayerCount,
                ContextSize: _appSettings.LocalLLMSettings.ContextSize,
                MaxTokens: MaxTokens
            );
            var geminiModelSettings = new GeminiModelSettings(ApiKey);
            var modifier = KeyPattern.GetModifiers(CtrlModifier, AltModifier, ShiftModifier, WinModifier);
            var keyPattern = new KeyPattern(modifier, Key);
            var keySettings = new KeySettings(IsHotkeyEnabled, keyPattern);
            var newSettings = new AppSettings(
                AutoStart,
                ModelTypeName,
                keySettings,
                localModelSettings,
                geminiModelSettings
            );
            var modifiedSettings = _settingsService.SettingsUpdate(App.MainWindow, newSettings);
            if (!newSettings.Equals(modifiedSettings)){
                UpdateSettings(modifiedSettings);
                return false;
            }
            _settingsService.SaveSettings(newSettings);
            return true;
        }

        private bool IsValidSettings()
        {
            ILLMModelSettings modelSettings;
            if (ModelTypeName == ModelType.LocalLLM)
            {
                modelSettings = new LLMLocalModelSettings(
                    ModelPath: LLMModelPath,
                    GpuEnable: GpuEnabled,
                    GpuLayerCount: GpuLayerCount,
                    ContextSize: _appSettings.LocalLLMSettings.ContextSize,
                    MaxTokens: MaxTokens
                );
                return LocalLLMProvider.CheckSettingsIntegrity(modelSettings);
            }
            else if (ModelTypeName == ModelType.Gemini)
            {
                modelSettings = new GeminiModelSettings(ApiKey);
                // TODO: Implement GeminiProvider to check settings integrity
                return GeminiProvider.CheckSettingsIntegrity(modelSettings);
            }
            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}