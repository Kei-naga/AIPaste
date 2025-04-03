using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NLog;
using Windows.System;
using AIPaste.Models.LLMModels;
using AIPaste.Models.StartupServices;
using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;

namespace AIPaste.ViewModels
{
    public partial class SettingsPageViewModel : INotifyPropertyChanged
    {
        private IAppSettings _appSettings;
        private readonly ISettingsService _settingsService;
        private readonly IStartupManager _startupManager;
        private readonly ITextCorrectorFactory _textCorrectorFactory;

        private readonly ILogger _logger;

        public SettingsPageViewModel(
            ISettingsService? settingsService = null, 
            IStartupManager? startupManager =null, 
            ITextCorrectorFactory? textCorrectorFactory = null,
            ILogger? logger = null )
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();
            _logger.Trace("SettingsPageViewModel created");
            _settingsService = settingsService ?? SettingsService.GetInstance();
            _appSettings = _settingsService.LoadSettings();
            _startupManager = startupManager ?? new StartupManager();
            _textCorrectorFactory = textCorrectorFactory ?? new TextCorrectorFactory();

            UpdateSettingsOnView();
        }

        private void UpdateSettingsOnView(AppSettings? newSettings = null)
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
                }
            }
        }

        private AppSettings GetCurrentAppSettings()
        {
            var localModelSettings = new LLMLocalModelSettings(
                ModelPath: LLMModelPath,
                GpuEnable: GpuEnabled,
                GpuLayerCount: GpuLayerCount,
                MaxContextSize: _appSettings.LocalLLMSettings.MaxContextSize,
                MaxTokens: MaxTokens
            );
            var geminiModelSettings = new GeminiModelSettings(ApiKey);
            var modifier = KeyPattern.GetModifiers(CtrlModifier, AltModifier, ShiftModifier, WinModifier);
            var keyPattern = new KeyPattern(modifier, Key);
            var keySettings = new KeySettings(IsHotkeyEnabled, keyPattern);
            return new AppSettings(
                AutoStart,
                ModelTypeName,
                keySettings,
                localModelSettings,
                geminiModelSettings
            );
        }

        public bool SaveSettings()
        {
            var newSettings = GetCurrentAppSettings();

            if (newSettings.Equals(_appSettings))
            {
                _logger.Trace("No settings changed, not saving");
                return true;
            }

            if (!IsValidLlmSettings(newSettings))
            {
                _logger.Warn("Invalid llm Settings");
                UpdateSettingsOnView();
                return false;
            }

            if (!ApplyHostkeySettings(newSettings))
            {
                _logger.Error("Failed to apply hotkey settings");
                UpdateSettingsOnView();
                return false;
            }

            if (!ApplyAutoStartSettings(newSettings))
            {
                _logger.Error("Failed to apply auto start settings");
                UpdateSettingsOnView();
                return false;
            }

            _logger.Info("Saving new Settings");
            try
            {
                _settingsService.SaveSettings(newSettings);
                if (newSettings.ModelType != ModelType.LocalLLM)
                {
                    _logger.Trace("Disposing LocalLlmModel");
                    LocalLlmSingleton.Dispose();
                }
                UpdateSettingsOnView(newSettings);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to save settings. Failing back.");
                _settingsService.SaveSettings(_appSettings);
                return false;
            }
            return true;
        }

        private bool IsValidLlmSettings(AppSettings appSettings)
        {
            try
            {
                var textCorrector = _textCorrectorFactory.CreateLlmTextCorrector(appSettings);
                if (!textCorrector.CheckIntegrity())
                {
                    throw new Exception("Failed to generate text");
                }
                return true;
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Is invalid llm settings");
                return false;
            }
        }

        private bool ApplyHostkeySettings(AppSettings newSettings)
        {
            if (!App.MainWindow?.ViewModel.UpdateHotkeySettings(newSettings.KeySettings) ?? false)
            {
                _appSettings.KeySettings = new KeySettings(false, newSettings.KeySettings.KeyPattern);
                return false;
            }
            return true;
        }

        private bool ApplyAutoStartSettings(AppSettings newSettings)
        {
            try { AutoStartToggleChanged(newSettings.AutoStart); }
            catch (Exception e)
            {
                _logger.Warn(e);
                _appSettings.AutoStart = false;
                return false;
            }
            return true;
        }

        private async void AutoStartToggleChanged(bool changedStatus)
        {
            await _startupManager.ToggleStartupAsync(changedStatus);
            var actualState = await _startupManager.IsAutoStartupMode();
            if (changedStatus != actualState)
            {
                throw new Exception("Failed to set AutoStart");
            }
            _logger.Info($"AutoStart set to {changedStatus}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}