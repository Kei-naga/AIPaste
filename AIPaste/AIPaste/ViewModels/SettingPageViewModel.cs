using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.System;
using AIPaste.Models.LLMModels;
using AIPaste.Models.StartupServices;
using AIPaste.Models.SettingsServices;
using AIPaste.Models.BackgroudServices;
using System.Threading.Tasks;
using AIPaste.common;
using AIPaste.Models.DTO;

namespace AIPaste.ViewModels
{
    public partial class SettingsPageViewModel : INotifyPropertyChanged
    {
        private IAppSettings _appSettings;
        private readonly ISettingsService _settingsService;
        private readonly IAutoStartupManager _startupManager;
        private readonly ITextCorrectorFactory _textCorrectorFactory;
        private readonly IHotKeyManager _hotKeyManager;

        private readonly IMyLogger _logger;

        public SettingsPageViewModel(
            ISettingsService? settingsService = null, 
            IAutoStartupManager? startupManager =null, 
            ITextCorrectorFactory? textCorrectorFactory = null,
            IHotKeyManager? hotKeyManager = null,
            IMyLogger? logger = null )
        {
            _logger = logger ??MyLogger.GetInstance();
            _logger.Trace("SettingsPageViewModel created");
            _settingsService = settingsService ?? SettingsService.GetInstance();
            _appSettings = _settingsService.LoadSettings();
            _startupManager = startupManager ?? new AutoStartupManager();
            _textCorrectorFactory = textCorrectorFactory ?? new TextCorrectorFactory();
            _hotKeyManager = (hotKeyManager ?? HotKeyManager.GetInstance()) 
                ?? throw new Exception("HotKeyManager is not initialized");

            UpdateSettingsOnView();
        }

        private void UpdateSettingsOnView(AppSettings? newSettings = null)
        {
            if (newSettings != null)
            {
                _appSettings = newSettings;
            }
            var localLlmSettings = GetLocalLlmSettings();
            LLMModelPath = localLlmSettings.ModelPath;
            GpuLayerCount = localLlmSettings.GpuLayerCount;
            MaxTokens = localLlmSettings.MaxTokens;
            GpuEnabled = localLlmSettings.GpuEnabled;
            IsHotkeyEnabled = _appSettings.KeySettings.IsHotkeyEnabled;
            Key = _appSettings.KeySettings.KeyPattern.Key;
            CtrlModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_CONTROL);
            AltModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_ALT);
            ShiftModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_SHIFT);
            WinModifier = _appSettings.KeySettings.KeyPattern.Modifiers.HasFlag(HOT_KEY_MODIFIERS.MOD_WIN);
            AutoStart = _appSettings.AutoStart;
            ModelTypeName = _appSettings.ActiveModelType;
            var geminiModelSettings = _appSettings.ModelSettingsList
                .FirstOrDefault(x => x is GeminiModelSettings) as GeminiModelSettings
                ?? throw new Exception("Gemini settings not found");
            ApiKey = geminiModelSettings.ApiKey;
        }

        private LlmLocalModelSettings GetLocalLlmSettings()
        {
            var localLlmSettings = _appSettings.ModelSettingsList
                .FirstOrDefault(x => x is LlmLocalModelSettings) as LlmLocalModelSettings
                ?? throw new Exception("Local LLM settings not found");
            return localLlmSettings;
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
            var localLlmSettings = GetLocalLlmSettings();
            var localModelSettings = new LlmLocalModelSettings(
                ModelPath: LLMModelPath,
                GpuEnable: GpuEnabled,
                GpuLayerCount: GpuLayerCount,
                MaxContextSize: localLlmSettings.MaxContextSize,
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
                [localModelSettings, geminiModelSettings]
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
                _logger.Warn("INVALID_LLM_SETINGS");
                UpdateSettingsOnView();
                return false;
            }

            if (!ApplyHostkeySettings(newSettings))
            {
                _logger.Error("FAILED_TO_APPLY_HOTKEY");
                UpdateSettingsOnView();
                return false;
            }

            if (!ApplyAutoStartSettings(newSettings))
            {
                _logger.Error("FAILED_TO_APPLY_AUTOSTART");
                UpdateSettingsOnView();
                return false;
            }

            try
            {
                _settingsService.SaveSettings(newSettings);
                if (newSettings.ActiveModelType != ModelType.LocalLLM)
                {
                    _logger.Trace("Disposing LocalLlmModel");
                    LocalLlmSingleton.Dispose();
                }
                UpdateSettingsOnView(newSettings);
                _logger.Info("SUCCESS_SAVING");
            }
            catch (Exception e)
            {
                _logger.Error("FAILING_BACK_SETTINGS");
                _logger.Debug(e);
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
            try
            {
                _hotKeyManager.UpdateHotkeySettings(newSettings.KeySettings);
                return true;
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Failed to apply hotkey settings");
                _hotKeyManager.UnRegisterHotKey();
                var newKeySettings = new KeySettings(false, newSettings.KeySettings.KeyPattern);
                _appSettings.KeySettings = newKeySettings;
                return false;
            }
        }

        private bool ApplyAutoStartSettings(AppSettings newSettings)
        {
            var result = Task.Run(() =>
                {
                    return AutoStartToggleChanged(newSettings.AutoStart);
                }).GetAwaiter().GetResult();
            if (!result)
            {
                _appSettings.AutoStart = false;
                return false;
            }
            _logger.Trace($"AutoStart set to {newSettings.AutoStart}");
            return true;
        }

        private async Task<bool> AutoStartToggleChanged(bool changedStatus)
        {
            await _startupManager.ToggleStartupAsync(changedStatus);
            var actualState = await _startupManager.IsAutoStartupMode();
            if (changedStatus != actualState)
            {
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}